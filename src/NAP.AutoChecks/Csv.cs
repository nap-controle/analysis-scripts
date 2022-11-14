using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace NAP.AutoChecks;

internal static class Csv
{
    public static async Task WriteAsync<T>(string file, IEnumerable<T> items)
    {
        await using var outputStream = File.Open(file, FileMode.Create);
        await using var textWriter = new StreamWriter(outputStream);
        var csvWriter = new CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ","
        });
        await csvWriter.WriteRecordsAsync(items);
    }
}