using CsvHelper.Configuration.Attributes;

namespace NAP.AutoChecks.Domain;

public class Stakeholder_SSTP
{
    /// <summary>
    /// The id.
    /// </summary>
    [Index(0)]
    public string Id { get; set; }
    
    /// <summary>
    /// The data supplier property.
    /// </summary>
    [Index(1)]
    public string DataSupplier { get; set; }
    
    /// <summary>
    /// The region
    /// </summary>
    [Index(2)]
    public string IsSSTP { get; set; }
}