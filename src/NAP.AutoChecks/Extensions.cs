using System.Globalization;

namespace NAP.AutoChecks;

internal static class Extensions
{
    private static readonly Random Rng = new ();  

    public static void Shuffle<T>(this IList<T> list)  
    {  
        var n = list.Count;  
        while (n > 1) {  
            n--;  
            var k = Rng.Next(n + 1);  
            (list[k], list[n]) = (list[n], list[k]);
        }  
    }
    
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
    {
        var list = new List<T>();
        await foreach (var item in asyncEnumerable)
        {
            list.Add(item);
        }

        return list;
    } 
    
    /// <summary>
    /// Returns a string representing the object in a culture invariant way.
    /// </summary>
    public static string? ToInvariantString(this object obj)
    {
        return obj is IConvertible convertible ? convertible.ToString(CultureInfo.InvariantCulture)
            : obj is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture)
            : obj.ToString();
    }

}