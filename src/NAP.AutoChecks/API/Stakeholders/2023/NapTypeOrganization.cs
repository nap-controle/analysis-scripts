using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace NAP.AutoChecks.API.Stakeholders._2023;

public class NapTypeOrganization
{
    /// <summary>
    /// The id.
    /// </summary>
    [Index(0)]
    public string Organization { get; set; }
    
    /// <summary>
    /// The name.
    /// </summary>
    [Index(1)]
    public string OrganizationName { get; set; }
    
    // /// <summary>
    // /// The type, if any.
    // /// </summary>
    // [Index(2)]
    // public string? Type { get; set; }

    internal static async Task<IEnumerable<NapTypeOrganization>> Load(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            MissingFieldFound = null
        };
        using var streamReader = new StreamReader(stream);
        using var csv = new CsvReader(streamReader, config);
        return await csv.GetRecordsAsync<NapTypeOrganization>().ToListAsync();
    }
}