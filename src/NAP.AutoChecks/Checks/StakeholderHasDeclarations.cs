using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Checks;

public class StakeholderHasDeclarations
{
    private readonly DataHandler _dataHandler;

    public StakeholderHasDeclarations(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders();

        var organizations = await _dataHandler.GetOrganizations();
        var results = new List<StakeholderHasDeclarationsResult>();
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

            if (stakeholder.IsMMTIS && !organization.HasMMTISDeclaration())
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration MMTIS"));
            }

            if (stakeholder.IsRTTI && !organization.HasRTTIDeclaration())
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration RTTI"));
            }

            if (stakeholder.IsSRTI && !organization.HasSRTIDeclaration())
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration SRTI"));
            }

            if (stakeholder.IsSSTP && !organization.HasSSTPDeclaration())
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration SSTP"));
            }
        }

        await _dataHandler.WriteResultAsync("stakeholders_no_declarations.csv", results);
    }
}