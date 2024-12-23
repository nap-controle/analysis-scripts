using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Evaluation1_2._2022;

public class SelectedIn2022Dataset
{
    [Index(0)]
    public string Organization { get; set; }
    
    [Index(1)]
    public string Package { get; set; }

    [Index(2)]
    public string NAPType { get; set; }

    internal static async Task<IEnumerable<SelectedIn2022Dataset>> Load(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };
        using var streamReader = new StreamReader(stream);
        using var csv = new CsvReader(streamReader, config);
        return await csv.GetRecordsAsync<SelectedIn2022Dataset>().ToListAsync();
    }

    public NAPType GetNAPType()
    {
        return this.NAPType switch
        {
            "MMTIS" => Domain.NAPType.MMTIS,
            "SRTI" => Domain.NAPType.SRTI,
            "RTTI" => Domain.NAPType.RTTI,
            "SSTP" => Domain.NAPType.SSTP,
            _ => throw new Exception("Cannot determine NAPType")
        };
    }
}