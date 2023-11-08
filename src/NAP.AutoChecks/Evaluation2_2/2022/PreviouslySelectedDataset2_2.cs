using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Evaluation2_2._2022;

public class PreviouslySelectedDataset2_2
{
    [Index(6)]
    public string OrganizationId { get; set; }
    
    [Index(2)]
    public bool SelectedMMTIS { get; set; }
    
    [Index(3)]
    public bool SelectedSRTI { get; set; }
    
    [Index(4)]
    public bool SelectedRTTI { get; set; }
    
    [Index(5)]
    public bool SelectedSSTP { get; set; }

    internal static async Task<IEnumerable<PreviouslySelectedDataset2_2>> Load(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };
        using var streamReader = new StreamReader(stream);
        using var csv = new CsvReader(streamReader, config);
        return await csv.GetRecordsAsync<PreviouslySelectedDataset2_2>().ToListAsync();
    }
}