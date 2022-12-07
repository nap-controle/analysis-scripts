using Microsoft.Extensions.Logging;
using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;
// ReSharper disable PossibleMultipleEnumeration

// ReSharper disable InconsistentNaming

namespace NAP.AutoChecks.Sampling;

public class RandomizeDatasets
{
    private readonly DataHandler _dataHandler;
    private readonly ILogger<RandomizeDatasets> _logger;
    private readonly int _maxMMTIS = 25;
    private readonly int _maxSRTI = 5;
    private readonly int _maxRTTI = 5;
    private readonly int _maxSSTP = 5;

    public RandomizeDatasets(DataHandler dataHandler, ILogger<RandomizeDatasets> logger)
    {
        _dataHandler = dataHandler;
        _logger = logger;
    }

    public async Task Run()
    {
        var stakeholders = await _dataHandler.GetStakeholders();

        var organizations = await _dataHandler.GetOrganizations();

        var packages = (await _dataHandler.GetPackages()).ToList();

        var randomizedStakeholders = stakeholders.ToList();
        randomizedStakeholders.Shuffle();

        var results = new List<RandomizeDatasetResult>();

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

            var hasSRTI = false;
            var hasMMTIS = false;
            var hasSSTP = false;
            var hasRTTI = false;
            foreach (var package in packagesForOrg)
            {
                if (package.NAP_type == null) continue;

                hasRTTI = hasRTTI || package.IsRTTI();
                hasSSTP = hasSSTP || package.IsSSTP();
                hasMMTIS = hasMMTIS || package.IsMMTIS();
                hasSRTI = hasSRTI || package.IsSRTI();
            }

            foreach (var package in packagesForOrg)
            {
                results.Add(new RandomizeDatasetResult(stakeholder, organization, package, hasRTTI, hasSRTI, hasSSTP,
                    hasMMTIS));
            }
        }

        // select first MMTIS packages per category.
        var datasetBudget = 0;
        var selectedSSTP = 0;
        while (selectedSSTP < _maxSSTP)
        {
            var next = this.SelectNextSSTP(results);
            if (next == null)
            {
                _logger.LogWarning(
                    "Could select only {Count} of {Max} organizations for SSTP",
                    selectedSSTP, _maxSSTP);
                datasetBudget += _maxSSTP - selectedSSTP;
                break;
            };

            selectedSSTP++;
            _logger.LogInformation(
                "Selected package {Number} {PackageName} for {OrganizationName}, for SSTP",
                selectedSSTP, next.PackageName, next.OrganizationName);
        }
        
        var selectedSRTI = 0;
        while (selectedSRTI < _maxSRTI)
        {
            var next = this.SelectNextSRTI(results);
            if (next == null)
            {
                _logger.LogWarning(
                    "Could select only {Count} of {Max} organizations for SRTI",
                    selectedSRTI, _maxSRTI);
                datasetBudget += _maxSRTI - selectedSRTI;
                break;
            }

            selectedSRTI++;
            _logger.LogInformation(
                "Selected package {Number} {PackageName} for {OrganizationName}, for SRTI",
                selectedSRTI, next.PackageName, next.OrganizationName);
        }
        
        var selectedRTTI = 0;
        while (selectedRTTI < _maxRTTI)
        {
            var next = this.SelectNextRTTI(results);
            if (next == null)
            {
                _logger.LogWarning(
                    "Could select only {Count} of {Max} organizations for RTTI",
                    selectedRTTI, _maxRTTI);
                datasetBudget += _maxRTTI - selectedRTTI;
                break;
            };

            selectedRTTI++;
            _logger.LogInformation(
                "Selected package {Number} {PackageName} for {OrganizationName}, for RTTI",
                selectedRTTI, next.PackageName, next.OrganizationName);
        }

        // select first MMTIS packages per category.
        if (datasetBudget > 0)
        {
            _logger.LogWarning(
                "There are {Extras} extra organizations to be selected, adding them to MMTIS",
                datasetBudget);
        }
        
        var selectedMMTIS = new Dictionary<MMTISType, int>();
        while (selectedMMTIS.Sum(x => x.Value) < _maxMMTIS + datasetBudget)
        {
            var hasSelected = false;
            foreach (var mmtisType in Enum.GetValues<MMTISType>())
            {
                if (!selectedMMTIS.TryGetValue(mmtisType, out var count))
                {
                    count = 0;
                    selectedMMTIS[mmtisType] = count;
                }

                var next = this.SelectNextMMTIS(results, mmtisType);
                if (next == null) continue;

                hasSelected = true;
                count++;
                selectedMMTIS[mmtisType] = count;
                
                _logger.LogInformation("Selected package {Number} {PackageName} for {OrganizationName}, number  {Count} for MMTIS of type {MmtisType}",
                    selectedMMTIS.Sum(x => x.Value), next.PackageName, next.OrganizationName, count, mmtisType);
            }

            if (hasSelected)
            {
                continue;
            }
            
            _logger.LogWarning("All possible MMTIS organizations selected");
            break;
        } 

        await _dataHandler.WriteResultAsync("randomized_datasets_by_nap_type.xlsx", results);
    }

    private RandomizeDatasetResult? SelectNextMMTIS(IEnumerable<RandomizeDatasetResult> results, MMTISType type)
    {
        foreach (var orgAndPackage in results)
        {
            if (!orgAndPackage.OrgHasMMTISPackage) continue;
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

    private RandomizeDatasetResult? SelectNextSSTP(IEnumerable<RandomizeDatasetResult> results)
    {
        foreach (var orgAndPackage in results)
        {
            if (!orgAndPackage.OrgHasSSTPPackage) continue;

            // get org packages.
            var packages = results.Where(x => x.OrganizationId == orgAndPackage.OrganizationId).ToList();
            
            // if already selected, skip.
            if (packages.Any(x => x.SelectedSSTP)) continue;
            
            // select first mmtis package in org.
            var first = packages.FirstOrDefault(x => x.PackageIsSSTP);
            if (first == null)
            {
                _logger.LogError("Organization has an MMTIS package but no package found for selection");
                continue;
            }

            first.SelectedSSTP = true;

            return first;
        }

        return null;
    }
    private RandomizeDatasetResult? SelectNextSRTI(IEnumerable<RandomizeDatasetResult> results)
    {
        foreach (var orgAndPackage in results)
        {
            if (!orgAndPackage.OrgHasSRTIPackage) continue;

            // get org packages.
            var packages = results.Where(x => x.OrganizationId == orgAndPackage.OrganizationId).ToList();
            
            // if already selected, skip.
            if (packages.Any(x => x.SelectedSRTI)) continue;
            
            // select first mmtis package in org.
            var first = packages.FirstOrDefault(x => x.PackageIsSRTI);
            if (first == null)
            {
                _logger.LogError("Organization has an MMTIS package but no package found for selection");
                continue;
            }

            first.SelectedSRTI = true;

            return first;
        }

        return null;
    }
    private RandomizeDatasetResult? SelectNextRTTI(IEnumerable<RandomizeDatasetResult> results)
    {
        foreach (var orgAndPackage in results)
        {
            if (!orgAndPackage.OrgHasRTTIPackage) continue;

            // get org packages.
            var packages = results.Where(x => x.OrganizationId == orgAndPackage.OrganizationId).ToList();
            
            // if already selected, skip.
            if (packages.Any(x => x.SelectedRTTI)) continue;
            
            // select first mmtis package in org.
            var first = packages.FirstOrDefault(x => x.PackageIsRTTI);
            if (first == null)
            {
                _logger.LogError("Organization has an MMTIS package but no package found for selection");
                continue;
            }

            first.SelectedRTTI = true;

            return first;
        }

        return null;
    }
}