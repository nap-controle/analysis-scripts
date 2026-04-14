using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace NAP.AutoChecks.API.Stakeholders._2025;

public class StakeholderCsv2025
{
    [Index(0)]
    public string Id { get; set; }

    [Index(1)]
    public string Name { get; set; }

    [Index(2)]
    public string OrganizationId { get; set; }

    internal static async Task<IEnumerable<StakeholderCsv2025>> Load(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            MissingFieldFound = null
        };
        using var streamReader = new StreamReader(stream);
        using var csv = new CsvReader(streamReader, config);
        return await csv.GetRecordsAsync<StakeholderCsv2025>().ToListAsync();
    }
}