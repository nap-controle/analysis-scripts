using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Queries;

public class StakeholdersPerNapType
{
    private readonly DataHandler _dataHandler;

    public StakeholdersPerNapType(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task GetDeclarations()
    {
        var stakeholders = await _dataHandler.GetStakeholders();

        var organizations = await _dataHandler.GetOrganizations();
        var results = new List<StakeholderAndNapType>();
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

            var result = new StakeholderAndNapType(stakeholder);
            if (organization.agreement_declaration_mmtis is { Length: > 0 } &&
                organization.agreement_declaration_mmtis[0] == "Y")
            {
                result.HasMMTISDeclaration = true;
            }

            if (!string.IsNullOrWhiteSpace(organization.rtti_doc_document_upload))
            {
                result.HasRTTSDeclaration = true;
            }

            if (!string.IsNullOrWhiteSpace(organization.srti_doc_document_upload))
            {
                result.HasSRTIDeclaration = true;
            }

            if (!string.IsNullOrWhiteSpace(organization.sstp_doc_document_upload))
            {
                result.HasSSTPDeclaration = true;
            }
        }

        await _dataHandler.WriteResultAsync("stakeholder_declarations.csv", results);
    }
}