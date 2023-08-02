using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace NAP.AutoChecks.Evaluation1_2._2022;

public class PreviouslySelectedDataset
{
    [Index(0)]
    public string Organization { get; set; }
    
    [Index(1)]
    public string Dataset { get; set; }

    [Index(2)]
    public string NAPType { get; set; }

    internal static async Task<IEnumerable<PreviouslySelectedDataset>> Load(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };
        using var streamReader = new StreamReader(stream);
        using var csv = new CsvReader(streamReader, config);
        return await csv.GetRecordsAsync<PreviouslySelectedDataset>().ToListAsync();
    }
}