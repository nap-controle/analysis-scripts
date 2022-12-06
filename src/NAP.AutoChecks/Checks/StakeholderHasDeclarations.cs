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
        
        var packages = (await _dataHandler.GetPackages()).ToList();

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

            if ((stakeholder.IsMMTIS || organization.HasMMTISPackage(packages)) && !organization.HasMMTISDeclaration())
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration MMTIS", organization, packages));
            }

            if ((stakeholder.IsRTTI || organization.HasSRTIPackage(packages)) && !organization.HasRTTIDeclaration())
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration RTTI", organization, packages));
            }

            if ((stakeholder.IsSRTI || organization.HasSRTIPackage(packages)) && !organization.HasSRTIDeclaration())
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration SRTI", organization, packages));
            }

            if ((stakeholder.IsSSTP || organization.HasSSTPPackage(packages)) && !organization.HasSSTPDeclaration())
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration SSTP", organization, packages));
            }
        }

        await _dataHandler.WriteResultAsync("stakeholders_no_declarations.csv", results);
    }
}