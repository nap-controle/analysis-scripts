using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Evaluation2_1;

public class StakeholdersWithoutDeclarations
{
    private readonly DataHandler _dataHandler;

    public StakeholdersWithoutDeclarations(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders();        
        
        var packages = (await _dataHandler.GetPackages()).ToList();

        var organizations = await _dataHandler.GetOrganizations();
        var results = new List<StakeholdersWithoutDeclarationsResult>();
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

            var error = string.Empty;
            if ((stakeholder.IsMMTIS || organization.HasMMTISPackage(packages)) && !organization.HasMMTISDeclaration())
            {
                error += "No agreement declaration MMTIS ";
            }

            if ((stakeholder.IsRTTI || organization.HasRTTIPackage(packages)) && !organization.HasRTTIDeclaration())
            {
                error += "No agreement declaration RTTI ";
            }

            if ((stakeholder.IsSRTI || organization.HasSRTIPackage(packages)) && !organization.HasSRTIDeclaration())
            {
                error += "No agreement declaration SRTI ";
            }

            if ((stakeholder.IsSSTP || organization.HasSSTPPackage(packages)) && !organization.HasSSTPDeclaration())
            {
                error += "No agreement declaration SSTP ";
            }
            
            results.Add(new StakeholdersWithoutDeclarationsResult(stakeholder, organization, packages, error,
                organization.HasMMTISDeclaration(), organization.HasRTTIDeclaration(), organization.HasSSTPDeclaration(), organization.HasSRTIDeclaration()));
        }

        await _dataHandler.WriteResultAsync("evaluation_2.1_stakeholders_no_declarations.xlsx", results);
    }
}