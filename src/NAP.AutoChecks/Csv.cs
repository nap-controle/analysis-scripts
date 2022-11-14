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

    public static async Task<IEnumerable<T>> ReadAsync<T>(string file)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };
        await using var stream = File.OpenRead(file);
        using var streamReader = new StreamReader(stream);
        using var csv = new CsvReader(streamReader, config);
        return await csv.GetRecordsAsync<T>().ToListAsync();
    }
}