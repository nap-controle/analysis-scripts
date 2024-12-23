using NAP.AutoChecks.API;

// ReSharper disable InconsistentNaming

namespace NAP.AutoChecks.Evaluation2_1;

public class StakeholdersWithDeclarations
{
    private readonly DataHandler _dataHandler;

    public StakeholdersWithDeclarations(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task Run()
    {
        var stakeholders = await _dataHandler.GetStakeholders();

        var organizations = await _dataHandler.GetOrganizations();

        var organizationsWithDeclarations = organizations.Where(x => x.HasRTTIDeclaration() || x.HasSRTIDeclaration() || x.HasSSTPDeclaration() ||
                                                                     x.HasMMTISDeclaration());

        var results = new List<StakeholdersWithDeclarationsResult>();
        
        foreach (var stakeholder in stakeholders)
        {
            if (stakeholder.ParsedOrganizationId == null)
            {
                //results.Add(new StakeholderHasPackagesResult(stakeholder, "no_organization_id"));
                continue;
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var organization = organizationsWithDeclarations.FirstOrDefault(x => x.Id == stakeholder.ParsedOrganizationId);
            if (organization == null)
            {
                continue;
            }
            
            results.Add(new StakeholdersWithDeclarationsResult(stakeholder, organization));
        }
        
        results.Shuffle();
        
        await _dataHandler.WriteResultAsync("evaluation_2.1_stakeholders_with_declarations.xlsx", results);
    }
}