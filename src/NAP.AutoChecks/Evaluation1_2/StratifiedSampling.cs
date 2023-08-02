using Microsoft.Extensions.Logging;
using NAP.AutoChecks.API;
using NAP.AutoChecks.API.Stakeholders._2023;
using NAP.AutoChecks.Domain;
using NAP.AutoChecks.Evaluation1_2._2022;

namespace NAP.AutoChecks.Evaluation1_2;

public class StratifiedSampling
{
    private readonly DataHandler _dataHandler;
    private readonly PreviouslySelectedDatasetLoader _previouslySelected;
    private readonly ILogger<StratifiedSampling> _logger;
    private readonly StratifiedSamplingSetting _setting;

    public StratifiedSampling(DataHandler dataHandler, ILogger<StratifiedSampling> logger, PreviouslySelectedDatasetLoader previouslySelected, StratifiedSamplingSetting setting)
    {
        _dataHandler = dataHandler;
        _logger = logger;
        _previouslySelected = previouslySelected;
        _setting = setting;
    }
    
    public async Task Run()
    {
        var stakeholders = await _dataHandler.GetStakeholders();

        var organizations = await _dataHandler.GetOrganizations();

        var packages = (await _dataHandler.GetPackages()).ToList();

        var randomizedStakeholders = stakeholders.ToList();
        randomizedStakeholders.Shuffle();
        
        // index previously selected.
        var previouslySelectedList = await _previouslySelected.Get();
        var previouslySelected = new PreviouslySelectedDatasets();
        foreach (var previouslySelectedDataset in previouslySelectedList)
        {
            var napType = previouslySelectedDataset.GetNAPType();
            var org = organizations.FirstOrDefault(x => x.Name == previouslySelectedDataset.Organization);
            if (org == null)
            {
                _logger.LogWarning("Cannot find previously selected organization");
            }
            else
            {
                previouslySelected.AddOrganization(napType, org);
            }

            var package = packages.FirstOrDefault(x => x.Name == previouslySelectedDataset.Package);
            if (package == null)
            {
                _logger.LogWarning("Cannot find previously selected package");
            }
            else
            {
                previouslySelected.AddPackage(napType, package);
            }
        }

        var results = new List<StratifiedSamplingResult>();

        foreach (var stakeholder in randomizedStakeholders)
        {
            if (stakeholder.ParsedOrganizationId == null)
            {
                //results.Add(new StakeholderHasPackagesResult(stakeholder, "no_organization_id"));
                continue;
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var organization = organizations.FirstOrDefault(x => x.Id == stakeholder.ParsedOrganizationId);
            if (organization == null)
            {
                continue;
            }

            var packagesForOrg = packages.Where(x => x.Organization.Id == organization.Id).ToList();
            packagesForOrg.Shuffle();

            foreach (var package in packagesForOrg)
            {
                results.Add(new StratifiedSamplingResult(stakeholder, organization, package));
            }
        }

        var types = new[] { NAPType.SSTP, NAPType.RTTI, NAPType.SRTI };
        var datasetBudget = 0;
        foreach (var type in types)
        {
            var target = _setting.SelectCountFor(type);
            _logger.LogInformation(
                "Started selecting {Count} dataset/organization pairs for {Type}",
                target, type);
            var  count = this.SelectNext(results, target, type, previouslySelected);
            datasetBudget += target - count;
        }
        
        // select first MMTIS packages per category.
        if (datasetBudget > 0)
        {
            _logger.LogWarning(
                "There are {Extras} extra organizations to be selected, adding them to MMTIS",
                datasetBudget);
        }
        //
        // var mmtisTarget = _setting.SelectCountFor(NAPType.MMTIS) + datasetBudget + 100;
        // _logger.LogInformation(
        //     "Started selecting {Count} dataset/organization pairs for {Type}",
        //     mmtisTarget, NAPType.MMTIS);
        // this.SelectNext(results, mmtisTarget, NAPType.MMTIS);
        
        var selectedMmtis = new Dictionary<MMTISType, int>();
        var full = new HashSet<MMTISType>();
        _logger.LogInformation(
            "Started selecting {Count} dataset/organization pairs for {Type}",
            _setting.SelectMMTIS + datasetBudget, NAPType.MMTIS);
        while (selectedMmtis.Sum(x => x.Value) < _setting.SelectMMTIS + datasetBudget)
        {
            var hasSelected = false;
            foreach (var mmtisType in Enum.GetValues<MMTISType>())
            {
                if (full.Contains(mmtisType)) continue;
                if (!selectedMmtis.TryGetValue(mmtisType, out var count))
                {
                    count = 0;
                    selectedMmtis[mmtisType] = count;
                }
        
                _logger.LogInformation("Selecting next for {MmtisType}", mmtisType);
                var next = this.SelectNextMMTIS(results, mmtisType);
                if (next == null)
                {
                    _logger.LogInformation("No more organization/package pairs available for {MmtisType}", mmtisType);
                    full.Add(mmtisType);
                    continue;
                }
        
                hasSelected = true;
                count++;
                selectedMmtis[mmtisType] = count;
                
                _logger.LogInformation("Selected package {Number} {PackageName} for {OrganizationName}, number  {Count} for MMTIS of type {MmtisType}",
                    selectedMmtis.Sum(x => x.Value), next.PackageName, next.OrganizationName, count, mmtisType);
            }
        
            if (hasSelected)
            {
                continue;
            }
            
            _logger.LogWarning("All possible MMTIS organizations selected");
            break;
        } 

        await _dataHandler.WriteResultAsync("evaluation_1.2_stratified-sampling.xlsx", results);
    }

    private StratifiedSamplingResult? SelectNextMMTIS(IEnumerable<StratifiedSamplingResult> results, MMTISType type)
    {
        foreach (var orgAndPackage in results)
        {
            if (!orgAndPackage.PackageIsFor(NAPType.MMTIS)) continue;
            if (orgAndPackage.StakeholderMMTIStype != type) continue;

            // get org packages.
            var packages = results.Where(x => x.OrganizationId == orgAndPackage.OrganizationId).ToList();
            
            // if already selected, skip.
            if (packages.Any(x => x.SelectedMMTIS)) continue;
            
            // select first mmtis package in org.
            var first = packages.FirstOrDefault(x => x.PackageIsMMTIS);
            if (first == null)
            {
                _logger.LogError("Organization has an MMTIS package but no package found for selection");
                continue;
            }

            first.SelectedMMTIS = true;

            return first;
        }

        return null;
    }

    /// <summary>
    /// Selects the first target packages in the results set that has the given NAPType and has a not previously selected organization.
    /// </summary>
    /// <param name="results">The result set.</param>
    /// <param name="target">The number of packages to try to select.</param>
    /// <param name="napType">The NAP type.</param>
    /// <param name="previouslySelected">The previously selected datasets.</param>
    /// <returns>The packages selected.</returns>
    /// <exception cref="Exception"></exception>
    private int SelectNext(IEnumerable<StratifiedSamplingResult> results, int target, NAPType napType, PreviouslySelectedDatasets previouslySelected)
    {
        //if (napType == NAPType.MMTIS) throw new Exception("Use the dedicated MMTIS method");
        
        var selected = 0;
        var modes = new[] { "org", "pack", "none" };
        foreach (var mode in modes)
        {
            foreach (var orgAndPackage in results)
            {
                if (selected >= target) return target;

                // make sure the package is for the requested type.
                if (!orgAndPackage.PackageIsFor(napType)) continue;

                _logger.LogInformation(
                    "Considering {PackageName} for {OrganizationName} for {Type}",
                    orgAndPackage.PackageName, orgAndPackage.OrganizationName, napType);
                
                // check if org or package has been considered before.
                var orgBefore = previouslySelected.OrganizationWasSelected(napType, orgAndPackage.OrganizationId);
                var packBefore = previouslySelected.PackageWasSelected(napType, orgAndPackage.PackageId);
                if (orgBefore)
                {
                    if (mode == "org")
                    {
                        _logger.LogInformation(
                            "Skipping {PackageName} for {OrganizationName} for {Type}, organization has been selected before",
                            orgAndPackage.PackageName, orgAndPackage.OrganizationName, napType);
                        continue;
                    }

                    _logger.LogInformation(
                        "{OrganizationName} has been selected before for {Type} but considering again",
                        orgAndPackage.OrganizationName, napType);
                }

                if (packBefore)
                {
                    if (mode == "pack")
                    {
                        _logger.LogInformation(
                            "Skipping {PackageName} for {OrganizationName} for {Type}, package has been selected before",
                            orgAndPackage.PackageName, orgAndPackage.OrganizationName, napType);
                        continue;
                    }

                    _logger.LogInformation(
                        "{PackageName} for {OrganizationName} has been selected before for {Type} but considering again",
                        orgAndPackage.PackageName, orgAndPackage.OrganizationName, napType);
                }

                // if org already selected, skip.
                var alreadySelectedPackage = results.FirstOrDefault(x => x.PackageSelectedFor(napType) &&
                                                                         x.OrganizationId ==
                                                                         orgAndPackage.OrganizationId);
                if (alreadySelectedPackage != null)
                {
                    _logger.LogInformation(
                        "Not selecting {PackageName}, {OrganizationName} already {PackageNameSelected} selected for {Type}",
                        orgAndPackage.PackageName, orgAndPackage.OrganizationName, alreadySelectedPackage.PackageName,
                        napType);
                    continue;
                }

                orgAndPackage.SetSelectedFor(napType);
                selected++;
                _logger.LogInformation(
                    "Selected package {Number} {PackageName} for {OrganizationName}, for {Type}",
                    selected, orgAndPackage.PackageName, orgAndPackage.OrganizationName, napType);
            }

            if (selected < target && mode == "org")
            {
                _logger.LogInformation(
                    "Only {Count} of {Target} selected, stopped ignoring previously selected organizations",
                    selected, target);
            }
            if (selected < target && mode == "pack")
            {
                _logger.LogInformation(
                    "Only {Count} of {Target} selected, stopped ignoring previously selected packages",
                    selected, target);
            }
        }

        if (selected <= target)
        {
            _logger.LogWarning(
                "All possible {Type} organizations selected",
                napType);
        }
        else
        {
            _logger.LogWarning(
                "Could select only {Count} of {Max} dataset/organization for {Type}",
                selected, _setting.SelectSSTP, napType);
        }
        return selected;
    }
}