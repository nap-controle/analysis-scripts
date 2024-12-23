using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation0_DataValidation;

public class AllOrganizations
{
    private readonly DataHandler _dataHandler;

    public AllOrganizations(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }
    
    public async Task Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders();
        var organizations = await _dataHandler.GetOrganizations();
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        var organizationFound = new List<AllOrganizationsResult>();
        foreach (var organization in organizations)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            var matchingStakeholder = stakeholders.FirstOrDefault(stakeholder => stakeholder.ParsedOrganizationId == organization.Id);

            organizationFound.Add(new AllOrganizationsResult(organization, matchingStakeholder));
        }

        await _dataHandler.WriteResultAsync("evaluation_0_organizations_matched_with_stakeholder.xlsx", organizationFound);
    }
}