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

            if (stakeholder.IsMMTIS)
            {
                if (organization.agreement_declaration_mmtis is { Length: > 0 } &&
                    organization.agreement_declaration_mmtis[0] == "Y")
                {
                }
                else
                {
                    results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration MMTIS"));
                }
            }

            if (stakeholder.IsRTTI && string.IsNullOrWhiteSpace(organization.rtti_doc_document_upload))
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration RTTI"));
            }

            if (stakeholder.IsSRTI && string.IsNullOrWhiteSpace(organization.srti_doc_document_upload))
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration SRTI"));
            }

            if (stakeholder.IsSSTP && string.IsNullOrWhiteSpace(organization.sstp_doc_document_upload))
            {
                results.Add(new StakeholderHasDeclarationsResult(stakeholder, "No agreement declaration SSTP"));
            }
        }

        await _dataHandler.WriteResultAsync("stakeholders_no_declarations.csv", results);
    }
}