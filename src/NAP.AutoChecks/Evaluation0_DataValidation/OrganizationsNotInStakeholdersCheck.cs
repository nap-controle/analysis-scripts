using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation0_DataValidation;

public class OrganizationsNotInStakeholdersCheck
{
    private readonly DataHandler _dataHandler;

    public OrganizationsNotInStakeholdersCheck(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }
    
    public async Task Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders();
        var organizations = await _dataHandler.GetOrganizations();
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        var organizationFound = new List<OrganizationsNotInStakeholdersResult>();
        foreach (var organization in organizations)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            var matchingStakeholder = stakeholders.FirstOrDefault(stakeholder => stakeholder.ParsedOrganizationId == organization.Id);

            if (matchingStakeholder == null)
            {
                organizationFound.Add(new OrganizationsNotInStakeholdersResult(organization));
            }
        }

        await _dataHandler.WriteResultAsync("evaluation_0_organizations_not_in_stakeholders.xlsx", organizationFound);
    }
}