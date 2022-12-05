using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Queries;

public class StakeholdersAllDeclarations
{
    private readonly DataHandler _dataHandler;

    public StakeholdersAllDeclarations(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task Get()
    {
        var stakeholders = await _dataHandler.GetStakeholders();

        var organizations = await _dataHandler.GetOrganizations();
        
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

            if (!string.IsNullOrWhiteSpace(organization.rtti_doc_document_upload))
            {
                var file = await _dataHandler.GetClient()
                    .DownloadDocument(organization.rtti_doc_document_upload, organization);
                await _dataHandler.WriteDeclarationDocumentForOrganizationAsync($"rtti_{organization.rtti_doc_document_upload}",
                    organization, file);
            }

            if (!string.IsNullOrWhiteSpace(organization.srti_doc_document_upload))
            {
                var file = await _dataHandler.GetClient()
                    .DownloadDocument(organization.srti_doc_document_upload, organization);
                await _dataHandler.WriteDeclarationDocumentForOrganizationAsync($"srti_{organization.srti_doc_document_upload}",
                    organization, file);
            }

            if (!string.IsNullOrWhiteSpace(organization.sstp_doc_document_upload))
            {
                var file = await _dataHandler.GetClient()
                    .DownloadDocument(organization.sstp_doc_document_upload, organization);
                await _dataHandler.WriteDeclarationDocumentForOrganizationAsync($"sstp_{organization.sstp_doc_document_upload}",
                    organization, file);
            }
        }
    }
}