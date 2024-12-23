using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Evaluation1_2._2023;

public class SelectedIn2023Dataset
{
    [Index(1)]
    public string OrganizationId { get; set; }
    
    [Index(2)]
    public string PackageId { get; set; }

    [Index(5)]
    public string SelectedMMTIS { get; set; }

    [Index(6)]
    public string SelectedSRTI { get; set; }

    [Index(7)]
    public string SelectedRTTI { get; set; }

    [Index(8)]
    public string SelectedSSTP { get; set; }

    internal static async Task<IEnumerable<SelectedIn2023Dataset>> Load(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };
        using var streamReader = new StreamReader(stream);
        using var csv = new CsvReader(streamReader, config);
        return await csv.GetRecordsAsync<SelectedIn2023Dataset>().ToListAsync();
    }

    public IEnumerable<NAPType> GetNAPTypes()
    {
        if (this.SelectedMMTIS == "TRUE") yield return NAPType.MMTIS;
        if (this.SelectedSRTI == "TRUE") yield return NAPType.SRTI;
        if (this.SelectedRTTI == "TRUE") yield return NAPType.RTTI;
        if (this.SelectedSSTP == "TRUE") yield return NAPType.SSTP;
    }
}