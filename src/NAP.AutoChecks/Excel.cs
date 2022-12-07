using System.Globalization;
using ClosedXML.Excel;

namespace NAP.AutoChecks;

public static class Excel
{
    public static void Write<T>(string file, IEnumerable<T> items)
    {
        var properties = typeof(T).GetProperties();
        var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("data");
        for (var p = 0; p < properties.Length; p++)
        {
            ws.Cell(1, p+1).Value = properties[p].Name;
        }

        var row = 2;
        foreach (var item in items)
        {
            for (var p = 0; p < properties.Length; p++)
            {
                ws.Cell(row, p+1).Value = properties[p].GetValue(item)?.ToInvariantString();
            }

            row++;
        }
        
        if (File.Exists(file)) File.Delete(file);
        wb.SaveAs(file);
    }
}