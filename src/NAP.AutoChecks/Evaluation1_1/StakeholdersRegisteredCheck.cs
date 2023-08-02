using NAP.AutoChecks.API;

namespace NAP.AutoChecks.Evaluation1_1;

public class StakeholdersRegisteredCheck
{
    private readonly DataHandler _dataHandler;

    public StakeholdersRegisteredCheck(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders();
        var organizations = await _dataHandler.GetOrganizations();

        var results = new List<StakeholdersRegisteredResult>();
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
            
            results.Add(new StakeholdersRegisteredResult(stakeholder, registered));
        }

        await _dataHandler.WriteResultAsync("evaluation_1.1_stakeholders_not_registered.xlsx", results);
    }
}