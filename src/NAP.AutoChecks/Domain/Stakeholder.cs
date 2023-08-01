using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace NAP.AutoChecks.Domain;

[DebuggerDisplay("{this.Id} [{this.OrganizationId}] {this.IsMMTIS} {this.IsSSTP} {this.IsRTTI} {this.IsSRTI}")]
public class Stakeholder
{
    /// <summary>
    /// The id.
    /// </summary>
    [Index(0)]
    public string Id { get; set; }

    /// <summary>
    /// The name.
    /// </summary>
    [Index(1)]
    public string Name { get; set; }
    
    /// <summary>
    /// The organization id, if any.
    /// </summary>
    [Index(5)]
    public string OrganizationId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [Ignore]
    public bool IsMMTIS { get; set; }
    
    /// <summary>
    /// The MMTIS type if any.
    /// </summary>
    [Ignore]
    public MMTISType? MMTISType { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [Ignore]
    public bool IsRTTI { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [Ignore]
    public bool IsSRTI { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [Ignore]
    public bool IsSSTP { get; set; }

    /// <summary>
    /// The parsed organization id, if any.
    /// </summary>
    public Guid? ParsedOrganizationId
    {
        get
        {
            if (!Guid.TryParse(this.OrganizationId, out var guid))
            {
                return null;
            }

            return guid;
        }
    }

    /// <summary>
    /// Loads the stakeholders list from a csv.
    /// </summary>
    /// <param name="stream">The stream with the csv.</param>
    /// <returns>The parsed stakeholders list.</returns>
    public static async Task<List<Stakeholder>> LoadFromCsv(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };
        using var streamReader = new StreamReader(stream);
        using var csv = new CsvReader(streamReader, config);
        return await csv.GetRecordsAsync<Stakeholder>().ToListAsync();
    }
}