using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace NAP.AutoChecks.API.Stakeholders._2023;

public class CkanId
{
    /// <summary>
    /// The id.
    /// </summary>
    [Index(0)]
    public string Organization { get; set; }

    /// <summary>
    /// The organization id.
    /// </summary>
    [Index(1)]
    public string OrganizationId { get; set; }

    internal static async Task<IEnumerable<CkanId>> Load(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };
        using var streamReader = new StreamReader(stream);
        using var csv = new CsvReader(streamReader, config);
        return await csv.GetRecordsAsync<CkanId>().ToListAsync();
    }
}