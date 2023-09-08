using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NAP.AutoChecks.API;
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

        // compile all possible packages to select but set then all to deselected.
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
        
        // SAMPLING1: 
        // - all packages for new organizations.
        // - all packages for organizations not in the 2022 list.
        
        // SAMPLING2:
        // - all packages with edits since previous sampling.
        // - all packages not previously checked.
        
        // PROCEDURE: 
        // - first select 5 for SSTP, RTTI and SRTI.
        // - after select remaining for SSTP using stratified sampling.
        // - use the SAMPLING1 filter.

        (bool select, string reason) Sampling1IncludeSample(StratifiedSamplingResult sample, NAPType type)
        {
            Debug.Assert(previouslySelected != null, nameof(previouslySelected) + " != null");
            
            // organization was not previously selected.
            // this also means new organizations.
            if (!previouslySelected.OrganizationWasSelected(type, sample.OrganizationId)) return (true, "organization never selected in previous sampling");

            return (false, "organization selected in previous sampling");
        }

        (bool select, string reason) Sampling2IncludeSample(StratifiedSamplingResult sample, NAPType type)
        {
            Debug.Assert(previouslySelected != null, nameof(previouslySelected) + " != null");
            
            // sample was modified after previous sampling day.
            if (sample.WasModifiedAfter(_setting.PreviousSamplingDay)) return (true, "modified after previous sampling");
            
            // sample was not checked before.
            if (!previouslySelected.PackageWasSelected(type, sample.PackageId)) return (true, "package was never selected before");

            return (false, "not modified and selected before");
        }
        
        // run SAMPLING1 first for all non-MMTIS.
        this.SelectNonMMTIS("SAMPLING1", Sampling1IncludeSample, results);
        // run SAMPLING2 first for all non-MMTIS.
        this.SelectNonMMTIS("SAMPLING2", Sampling2IncludeSample, results);
        
        // run SAMPLING1 for MMTIS.
        var types = new[] { NAPType.SSTP, NAPType.RTTI, NAPType.SRTI };
        // calculate extra dataset budget.
        var datasetBudget = 0;
        foreach (var type in types)
        {
            var target = _setting.SelectCountFor(type);
            var count = results.Count(x => x.PackageSelectedFor(type));
            datasetBudget += target - count;
        }
        if (datasetBudget > 0)
        {
            _logger.LogWarning(
                "There are {Extras} extra organizations to be selected, adding them to MMTIS",
                datasetBudget);
        }
        
        // run SAMPLING1 first for MMTIS.
        this.SelectMMTIS("SAMPLING1", Sampling1IncludeSample, results, datasetBudget);
        // run SAMPLING2 first for MMTIS.
        this.SelectMMTIS("SAMPLING2", Sampling2IncludeSample, results, datasetBudget);

        await _dataHandler.WriteResultAsync("evaluation_1.2_stratified-sampling.xlsx", results);
    }

    private void SelectNonMMTIS(string samplingName, Func<StratifiedSamplingResult, NAPType, (bool select, string reason)> includeFilter, List<StratifiedSamplingResult> results)
    {
        var types = new[] { NAPType.SSTP, NAPType.RTTI, NAPType.SRTI };
        foreach (var napType in types)
        {
            var selected = results.Count(x => x.PackageSelectedFor(napType));
            var target = _setting.SelectCountFor(napType);
            if (target - selected <= 0)
            {
                _logger.LogInformation(
                    "{SamplingName}: Already {Count} dataset/organization pairs selected for {Type}, not selecting any more",
                    samplingName, target, napType);
                continue;
            }
            
            _logger.LogInformation(
                "{SamplingName}: Started selecting {Count} dataset/organization pairs for {Type}",
                samplingName, target - selected, napType);
            
            foreach (var orgAndPackage in results)
            {
                if (selected >= target) break;

                // make sure the package is for the requested type.
                if (!orgAndPackage.PackageIsFor(napType)) continue;

                _logger.LogDebug(
                    "{SamplingName}: Considering {PackageName} for {OrganizationName} for {Type}",
                    samplingName, orgAndPackage.PackageName, orgAndPackage.OrganizationName, napType);
                
                // check filter.
                var (select, reason) = includeFilter(orgAndPackage, napType);
                if (!select)
                {
                    _logger.LogDebug(
                        "{SamplingName}: Skipping {PackageName} for {OrganizationName}, {Reason}",
                        samplingName, orgAndPackage.PackageName, orgAndPackage.OrganizationName, reason);
                    continue;
                }
                
                // if already selected, skip.
                var alreadySelectedPackage = results.FirstOrDefault(x => x.PackageSelectedFor(napType) &&
                                                                         x.OrganizationId ==
                                                                         orgAndPackage.OrganizationId);
                if (alreadySelectedPackage != null)
                {
                    _logger.LogDebug(
                        "{SamplingName}: Not selecting {PackageName}, {OrganizationName} already {PackageNameSelected} selected for {Type}",
                        samplingName, orgAndPackage.PackageName, orgAndPackage.OrganizationName, alreadySelectedPackage.PackageName,
                        napType);
                    continue;
                }

                orgAndPackage.SetSelectedFor(napType, reason);
                selected++;
                _logger.LogInformation(
                    "{SamplingName}: Selected package {Number} {PackageName} for {OrganizationName}, for {Type}, {Reason}",
                    samplingName, selected, orgAndPackage.PackageName, orgAndPackage.OrganizationName, napType, reason);
            }
        }
    }

    private void SelectMMTIS(string samplingName, Func<StratifiedSamplingResult, NAPType, (bool select, string reason)> includeFilter,
        List<StratifiedSamplingResult> results, int datasetBudget)
    {
        var napType = NAPType.MMTIS;
        var target = _setting.SelectMMTIS + datasetBudget;
        var selected = results.Count(x => x.PackageSelectedFor(NAPType.MMTIS));
        _logger.LogInformation(
            "{SamplingName}: Started selecting {Count} dataset/organization pairs for {Type}",
            samplingName, target - selected, NAPType.MMTIS);
        while (selected < target)
        {
            var hasSelected = false;
            foreach (var mmtisType in Enum.GetValues<MMTISType>())
            {
                foreach (var orgAndPackage in results)
                {
                    if (!orgAndPackage.PackageIsFor(NAPType.MMTIS)) continue;
                    if (orgAndPackage.StakeholderMMTIStype != mmtisType) continue;
                    
                    _logger.LogDebug(
                        "{SamplingName}: Considering {PackageName} for {OrganizationName} for {Type} and {MMTISType}",
                        samplingName, orgAndPackage.PackageName, orgAndPackage.OrganizationName, napType, mmtisType);
                
                    // check filter.
                    var (select, reason) = includeFilter(orgAndPackage, napType);
                    if (!select)
                    {
                        _logger.LogDebug(
                            "{SamplingName}: Skipping {PackageName} for {OrganizationName}, {Reason}",
                            samplingName, orgAndPackage.PackageName, orgAndPackage.OrganizationName, reason);
                        continue;
                    }

                    // if already selected, skip.
                    var alreadySelectedPackage = results.FirstOrDefault(x =>
                        x.OrganizationId == orgAndPackage.OrganizationId
                        && x.SelectedMMTIS && x.StakeholderMMTIStype == mmtisType);
                    if (alreadySelectedPackage != null)
                    {
                        _logger.LogDebug(
                            "{SamplingName}: Not selecting {PackageName}, {OrganizationName} already {PackageNameSelected} selected for {Type} and {MMTISType}",
                            samplingName, orgAndPackage.PackageName, orgAndPackage.OrganizationName, alreadySelectedPackage.PackageName,
                            napType, mmtisType);
                        continue;
                    }

                    // package can be selected.
                    orgAndPackage.SetSelectedFor(napType, reason);
                    selected = results.Count(x => x.PackageSelectedFor(NAPType.MMTIS));
                    var mmtisTypeCount = results.Count(x => x.PackageSelectedFor(NAPType.MMTIS) && x.StakeholderMMTIStype == mmtisType);
                    _logger.LogInformation(
                        "{SamplingName}: Selected package {Number} {PackageName} for {OrganizationName}, number {Count} for MMTIS of type {MmtisType}, {Reason}",
                        samplingName, selected, orgAndPackage.PackageName, orgAndPackage.OrganizationName, mmtisTypeCount, mmtisType, reason);
                }
            }
            
            if (!hasSelected) break;
        }
    }
}