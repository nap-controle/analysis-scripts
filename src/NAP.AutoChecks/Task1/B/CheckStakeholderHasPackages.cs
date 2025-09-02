using NAP.AutoChecks.API;
using NAP.AutoChecks.Evaluation1_1;

namespace NAP.AutoChecks.Task1.B;

public class CheckStakeholderHasPackages
{
    private readonly DataHandler _dataHandler;

    public CheckStakeholderHasPackages(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders();
        var organizations = await _dataHandler.GetOrganizations();
        var packages = await _dataHandler.GetPackages();

        var results = new List<CheckStakeholderHasPackagesResult>();
        foreach (var stakeholder in stakeholders)
        {
            if (stakeholder.ParsedOrganizationId == null)
            {
                results.Add(new CheckStakeholderHasPackagesResult(stakeholder, "no_organization_id"));
                continue;
            }
            
            // ReSharper disable once PossibleMultipleEnumeration
            var organization = organizations.FirstOrDefault(x => x.Id == stakeholder.ParsedOrganizationId);
            if (organization == null)
            {
                results.Add(new CheckStakeholderHasPackagesResult(stakeholder, "no_organization_matched"));
                continue;
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var package = packages.FirstOrDefault(x => x.Organization.Id == organization.Id);
            if (package == null)
            {
                results.Add(new CheckStakeholderHasPackagesResult(stakeholder, "no_package"));
                continue;
            }
        }

        await _dataHandler.WriteResultAsync("evaluation_1.1_stakeholders_without_package.xlsx", results);
    }
}