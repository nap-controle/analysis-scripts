using NAP.AutoChecks.API;

namespace NAP.AutoChecks.Queries;

public class OrganizationsGetProxyAgreements
{
    private readonly DataHandler _dataHandler;

    public OrganizationsGetProxyAgreements(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task Get()
    {
        var organizations = await _dataHandler.GetOrganizations();
        
        foreach (var organization in organizations)
        {
            if (!string.IsNullOrWhiteSpace(organization.proxy_pdf_url))
            {
                var file = await _dataHandler.GetClient()
                    .DownloadDocument(organization.proxy_pdf_url, organization);
                await _dataHandler.WriteProxyAgreementForOrganizationAsync($"{organization.proxy_pdf_url}",
                    organization, file);
            }
        }
    }
}