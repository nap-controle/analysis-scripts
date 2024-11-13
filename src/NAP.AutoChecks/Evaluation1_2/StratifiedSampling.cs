using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;
using NAP.AutoChecks.Evaluation1_2._2022;
using NAP.AutoChecks.Evaluation1_2._2023;

namespace NAP.AutoChecks.Evaluation1_2;

public class StratifiedSampling
{
    private readonly DataHandler _dataHandler;
    private readonly SelectedIn2022DatasetLoader _selected2022;
    private readonly SelectedIn2023DatasetLoader _selected2023;
    private readonly ILogger<StratifiedSampling> _logger;
    private readonly StratifiedSamplingSetting _setting;

    public StratifiedSampling(DataHandler dataHandler, ILogger<StratifiedSampling> logger, SelectedIn2022DatasetLoader selected2022, StratifiedSamplingSetting setting, SelectedIn2023DatasetLoader selected2023)
    {
        _dataHandler = dataHandler;
        _logger = logger;
        _selected2022 = selected2022;
        _setting = setting;
        _selected2023 = selected2023;
    }
    
    public async Task Run()
    {
        var stakeholders = await _dataHandler.GetStakeholders();

        var organizations = await _dataHandler.GetOrganizations();

        var packages = (await _dataHandler.GetPackages()).ToList();
        
        var randomizedStakeholders = stakeholders.ToList();
        randomizedStakeholders.Shuffle();
        
        // index previously selected in 2022.
        var previouslySelectedList2022 = await _selected2022.Get();
        var previouslySelected = new PreviouslySelectedDatasets();
        foreach (var previouslySelectedDataset in previouslySelectedList2022)
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
        
        // index previously selected in 2023.
        var previouslySelectedList2023 = await _selected2023.Get();
        foreach (var previouslySelectedDataset in previouslySelectedList2023)
        {
            if (!Guid.TryParse(previouslySelectedDataset.OrganizationId, out var organizationId))
                throw new Exception("Invalid guid");
            if (!Guid.TryParse(previouslySelectedDataset.PackageId, out var packageId))
                throw new Exception("Invalid guid");
            
            var napTypes = previouslySelectedDataset.GetNAPTypes();
            foreach (var napType in napTypes)
            {
                var org = organizations.FirstOrDefault(x => x.Id == organizationId);
                if (org == null)
                {
                    _logger.LogWarning("Cannot find previously selected organization");
                }
                else
                {
                    previouslySelected.AddOrganization(napType, org);
                }

                var package = packages.FirstOrDefault(x => x.Id == packageId);
                if (package == null)
                {
                    _logger.LogWarning("Cannot find previously selected package");
                }
                else
                {
                    previouslySelected.AddPackage(napType, package);
                }
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
        
        // SAMPLING1 - new orgs:
        // - all packages for new organizations.
        // - all packages for organizations not in the 2022 or 2023 list.
        
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

        (bool select, string reason) Sampling3IncludeSample(StratifiedSamplingResult sample, NAPType type)
        {
            return (true, "check previously checked packages again");
        }
        
        // run for all non-MMTIS.
        this.SelectNonMMTIS("NEW_ORGS", Sampling1IncludeSample, true, results);
        this.SelectNonMMTIS("NEW_MOD_PACK", Sampling2IncludeSample, true, results);
        this.SelectNonMMTIS("REST_MAX_2", Sampling3IncludeSample, true, results);
        this.SelectNonMMTIS("REST", Sampling3IncludeSample, false, results);
        
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
        
        // run for MMTIS.
        this.SelectMMTIS("NEW_ORGS", Sampling1IncludeSample,true, results, datasetBudget);
        this.SelectMMTIS("NEW_MOD_PACK", Sampling2IncludeSample,true, results, datasetBudget);
        this.SelectMMTIS("REST_MAX_2", Sampling3IncludeSample, true, results, datasetBudget);
        this.SelectMMTIS("REST", Sampling3IncludeSample, false, results, datasetBudget);

        await _dataHandler.WriteResultAsync("evaluation_1.2_stratified-sampling.xlsx", results);
    }

    private void SelectNonMMTIS(string samplingName, Func<StratifiedSamplingResult, NAPType, (bool select, string reason)> includeFilter, bool checkMax2,
        List<StratifiedSamplingResult> results)
    {
        var types = new[] { NAPType.SSTP, NAPType.RTTI, NAPType.SRTI };
        foreach (var napType in types)
        {
            var selected = results.Count(x => x.PackageSelectedFor(napType));
            var target = _setting.SelectCountFor(napType);
            if (target - selected <= 0)
            {
                _logger.LogInformation(
                    "{SamplingName}-{Type}: Already {Count} dataset/organization pairs selected, not selecting any more",
                    samplingName, napType, target);
                continue;
            }
            
            _logger.LogInformation(
                "{SamplingName}-{Type}: Started selecting {Count} dataset/organization pairs for",
                samplingName, napType, target - selected);
            
            foreach (var orgAndPackage in results)
            {
                if (selected >= target) break;

                // make sure the package is for the requested type.
                if (!orgAndPackage.PackageIsFor(napType)) continue;

                _logger.LogDebug(
                    "{SamplingName}-{Type}: Considering {PackageName}@{OrganizationName}",
                    samplingName, napType, orgAndPackage.PackageName, orgAndPackage.OrganizationName);
                
                // check filter.
                var (select, reason) = includeFilter(orgAndPackage, napType);
                if (!select)
                {
                    _logger.LogDebug(
                        "{SamplingName}-{Type}: Skipping {PackageName}@{OrganizationName}, {Reason}",
                        samplingName, napType, orgAndPackage.PackageName, orgAndPackage.OrganizationName, reason);
                    continue;
                }
                
                // if already selected, skip.
                if (orgAndPackage.PackageSelectedFor(napType))
                {
                    _logger.LogDebug(
                        "{SamplingName}-{Type}: Not selecting package {PackageName}@{OrganizationName}, package is already selected",
                        samplingName, napType, orgAndPackage.PackageName, orgAndPackage.OrganizationName);
                    continue;
                }
                
                // if already 2 selected, skip.
                if (checkMax2)
                {
                    var already2SelectedForOrg = results.Has2PackagesSelected(napType, orgAndPackage.OrganizationId);
                    if (already2SelectedForOrg)
                    {
                        _logger.LogDebug(
                            "{SamplingName}-{Type}: Not selecting {PackageName}@{OrganizationName} already 2 selected for organization",
                            samplingName, napType, orgAndPackage.PackageName, orgAndPackage.OrganizationName);
                        continue;
                    }

                    // if already 1 selected, log this.
                    var already1SelectedForOrg = results.HasPackageSelected(napType, orgAndPackage.OrganizationId);
                    if (already1SelectedForOrg)
                    {
                        reason += " and only 1 package already selected for org";
                    }
                }

                orgAndPackage.SetSelectedFor(napType, reason);
                selected++;
                _logger.LogInformation(
                    "{SamplingName}-{Type}: Selected package {Number} {PackageName}@{OrganizationName}, {Reason}",
                    samplingName, napType, selected, orgAndPackage.PackageName, orgAndPackage.OrganizationName, reason);
            }
        }
    }

    private void SelectMMTIS(string samplingName, Func<StratifiedSamplingResult, NAPType, (bool select, string reason)> includeFilter, bool checkMax2,
        List<StratifiedSamplingResult> results, int datasetBudget)
    {
        var napType = NAPType.MMTIS;
        var target = _setting.SelectMMTIS + datasetBudget;
        var selected = results.Count(x => x.PackageSelectedFor(NAPType.MMTIS));
        _logger.LogInformation(
            "{SamplingName}-{Type}: Started selecting {Count} dataset/organization pairs",
            samplingName, napType, target - selected);
        while (selected < target)
        {
            var hasSelected = false;
            // foreach (var mmtisType in Enum.GetValues<MMTISType>())
            // {
            foreach (var orgAndPackage in results)
            {
                if (!orgAndPackage.PackageIsFor(napType)) continue;
                //if (orgAndPackage.StakeholderMMTIStype != mmtisType) continue;

                _logger.LogDebug(
                    "{SamplingName}-{Type}: Considering {PackageName}@{OrganizationName}",
                    samplingName, napType, orgAndPackage.PackageName, orgAndPackage.OrganizationName);

                // check filter.
                var (select, reason) = includeFilter(orgAndPackage, napType);
                if (!select)
                {
                    _logger.LogDebug(
                        "{SamplingName}-{Type}: Skipping {PackageName}@{OrganizationName}, {Reason}",
                        samplingName, napType, orgAndPackage.PackageName, orgAndPackage.OrganizationName, reason);
                    continue;
                }

                // if already selected, skip.
                if (orgAndPackage.PackageSelectedFor(napType))
                {
                    _logger.LogDebug(
                        "{SamplingName}-{Type}: Not selecting package {PackageName}@{OrganizationName}, package already selected",
                        samplingName, napType, orgAndPackage.PackageName, orgAndPackage.OrganizationName);
                    continue;
                }

                if (checkMax2)
                {
                    // if already 2 selected, skip.
                    var already2SelectedForOrg = results.Has2PackagesSelected(napType, orgAndPackage.OrganizationId);
                    if (already2SelectedForOrg)
                    {
                        _logger.LogDebug(
                            "{SamplingName}-{Type}: Not selecting package {PackageName}@{OrganizationName}, already 2 selected for organization",
                            samplingName, napType, orgAndPackage.PackageName, orgAndPackage.OrganizationName);
                        continue;
                    }

                    // if already 1 selected, log this.
                    var already1SelectedForOrg = results.HasPackageSelected(napType, orgAndPackage.OrganizationId);
                    if (already1SelectedForOrg)
                    {
                        reason += " and only 1 package already selected for org";
                    }
                }

                // package can be selected.
                orgAndPackage.SetSelectedFor(napType, reason);
                selected = results.Count(x => x.PackageSelectedFor(napType));
                _logger.LogInformation(
                    "{SamplingName}-{Type}: Selected package {Number} {PackageName}@{OrganizationName}, number {Count} for MMTIS, {Reason}",
                    samplingName, napType, selected, orgAndPackage.PackageName, orgAndPackage.OrganizationName, selected,
                    reason);

                if (selected >= target) break;
            }
            //}

            if (!hasSelected) break;
        }
    }
}