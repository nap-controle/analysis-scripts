using NAP.AutoChecks.API;

namespace NAP.AutoChecks.Checks;

public class StakeholdersWithoutOrganization
{
    private readonly DataHandler _dataHandler;

    public StakeholdersWithoutOrganization(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders();
        var organizations = await _dataHandler.GetOrganizations();

        var results = new List<StakeholdersWithoutOrganizationResult>();
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
            
            results.Add(new StakeholdersWithoutOrganizationResult(stakeholder, registered));
        }

        await _dataHandler.WriteResultAsync("stakeholders_not_registered.xlsx", results);
    }
}