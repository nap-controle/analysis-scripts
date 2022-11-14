using CsvHelper.Configuration.Attributes;

namespace NAP.AutoChecks.Domain;

public class Stakeholder_MMTIS
{
    // Organisation ID;Data supplier;Regio;DA-stakeholder MMTIS
    
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
    public string Region { get; set; }
    
    /// <summary>
    /// The region
    /// </summary>
    [Index(3)]
    public string IsMMTIS { get; set; }
}