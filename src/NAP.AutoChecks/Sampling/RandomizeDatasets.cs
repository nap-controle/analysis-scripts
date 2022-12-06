using NAP.AutoChecks.API;
// ReSharper disable InconsistentNaming

namespace NAP.AutoChecks.Sampling;

public class RandomizeDatasets
{
    private readonly DataHandler _dataHandler;

    public RandomizeDatasets(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
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
                results.Add(new RandomizeDatasetResult(stakeholder, organization, package, hasRTTI, hasSRTI, hasSSTP, hasMMTIS));
            }
        }
        
        await _dataHandler.WriteResultAsync("randomized_datasets_by_nap_type.csv", results);
    }
}