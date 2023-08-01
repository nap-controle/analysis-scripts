using CsvHelper.Configuration.Attributes;

namespace NAP.AutoChecks.Domain;

public class CategorizedOrganization_MMTIS
{
    [Index(1)]
    public string OrganizationId { get; set; }
    
    [Index(3)]
    public string IsTransportAuthority { get; set; }

    [Index(4)]
    public string IsTransportOperator { get; set; }

    [Index(5)]
    public string IsTransportondemandserviceprovider { get; set; }

    [Index(6)]
    public string IsInfrastructureManager { get; set; }
}