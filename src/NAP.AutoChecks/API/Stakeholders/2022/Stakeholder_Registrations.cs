using CsvHelper.Configuration.Attributes;

namespace NAP.AutoChecks.Domain;

public class Stakeholder_Registrations
{
    [Index(0)]
    public string Id { get; set; }
    
    [Index(1)]
    public string Name { get; set; }
    
    [Index(2)]
    public string OrganizationId { get; set; }
}