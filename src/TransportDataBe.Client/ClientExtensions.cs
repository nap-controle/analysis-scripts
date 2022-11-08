using TransportDataBe.Client.Models;

namespace TransportDataBe.Client;

public static class ClientExtensions
{
    public static async Task<IEnumerable<Organization>> GetOrganizations(this Client client)
    {
        var organizationIds = await client.GetOrganizationList();
        var organizations = new List<Organization>();
        foreach (var organizationId in organizationIds.Result)
        {
            var org = await client.GetOrganization(organizationId);
            if (org.Success)
            {
                organizations.Add(org.Result);
            }
        }

        return organizations;
    }
}