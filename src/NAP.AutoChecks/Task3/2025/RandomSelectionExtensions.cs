using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Task3._2025;

public static class RandomSelectionExtensions
{
    public static IEnumerable<Package> SelectRandom(this IEnumerable<Package> packages, int max)
    {
        var packagesList = packages.ToList();
        packagesList.Shuffle();

        return packagesList.GetRange(0, Math.Min(max, packagesList.Count));
    }

    public static void MarkSelected(this Dictionary<Guid, HashSet<NAPType>> selected, IEnumerable<Package> packages,
        NAPType type)
    {
        foreach (var package in packages)
        {
            if (!selected.TryGetValue(package.Id, out var types))
            {
                types = new HashSet<NAPType>();
                selected[package.Id] = types;
            }

            types.Add(type);
        }
    }
}