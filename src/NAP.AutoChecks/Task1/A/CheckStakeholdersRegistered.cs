using NAP.AutoChecks.API;

namespace NAP.AutoChecks.Task1.A;

public class CheckStakeholdersRegistered
{
    private readonly DataHandler _dataHandler;

    public CheckStakeholdersRegistered(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<IEnumerable<CheckStakeholdersRegisteredResult>> Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders2025();
        var organizations = await _dataHandler.GetOrganizations();

        var results = new List<CheckStakeholdersRegisteredResult>();
        foreach (var stakeholder in stakeholders)
        {
            var registered = false;
            if (stakeholder.ParsedOrganizationId != null)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                var organization = organizations.FirstOrDefault(x => x.Id == stakeholder.ParsedOrganizationId);
                if (organization != null)
                {
                    registered = true;
                }
            }
            
            results.Add(new CheckStakeholdersRegisteredResult(stakeholder, registered));
        }

        return results;
    }
}