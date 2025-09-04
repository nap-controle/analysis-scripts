using NAP.AutoChecks.API;

namespace NAP.AutoChecks.Task0.Checks;

public class CheckOrganizationStakeholder
{
    private readonly DataHandler _dataHandler;

    public CheckOrganizationStakeholder(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }
    
    public async Task Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders();
        var organizations = await _dataHandler.GetOrganizations();
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        var organizationFound = new List<CheckOrganizationStakeholderResult>();
        foreach (var organization in organizations)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            var matchingStakeholder = stakeholders.FirstOrDefault(stakeholder => stakeholder.ParsedOrganizationId == organization.Id);
            if (matchingStakeholder != null) continue;
            
            organizationFound.Add(new CheckOrganizationStakeholderResult()
            {
                Id = organization.Id,
                Name = organization.Name,
            });
        }

        await _dataHandler.WriteResultAsync("task0_no-stakeholder.xlsx", organizationFound);
    }
}