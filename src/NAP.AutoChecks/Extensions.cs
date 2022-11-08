namespace NAP.AutoChecks;

internal static class Extensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
    {
        var list = new List<T>();
        await foreach (var item in asyncEnumerable)
        {
            list.Add(item);
        }

        return list;
    } 
}