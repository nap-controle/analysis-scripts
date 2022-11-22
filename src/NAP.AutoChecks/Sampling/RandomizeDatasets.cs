using NAP.AutoChecks.API;

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

        var results = new List<RandomizeDatasetResult>();

        foreach (var stakeholder in stakeholders)
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

            foreach (var package in packages)
            {
                if (package.Organization.Id != organization.Id) continue;
                
                results.Add(new RandomizeDatasetResult(stakeholder, organization, package));
            }
        }

        results.Shuffle();
        
        await _dataHandler.WriteResultAsync("randomized_datasets_by_nap_type.csv", results);
    }
}