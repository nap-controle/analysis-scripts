using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Task3._2025;

public static class PackageExtensions
{
    public static bool IsNAPType(this Package package, NAPType type)
    {
        switch (type)
        {
            case NAPType.MMTIS:
                return package.NAP_type != null &&
                       package.NAP_type.Any(x => x.ToLowerInvariant() == "mmtis");
            case NAPType.SRTI:
                return package.NAP_type != null &&
                       package.NAP_type.Any(x => x.ToLowerInvariant() == "srti");
            case NAPType.SSTP:
                return package.NAP_type != null &&
                       package.NAP_type.Any(x => x.ToLowerInvariant() == "sstp");
            case NAPType.RTTI:
                return package.NAP_type != null &&
                       package.NAP_type.Any(x => x.ToLowerInvariant() == "rtti");
        }

        throw new Exception("Unknown NAP type");
    }

    public static bool IsCandidate(this Package package, NAPType type,
        IReadOnlyDictionary<Guid, HashSet<NAPType>> selectedBefore)
    {
        // if not of nap type, it is not a candidate.
        if (!package.IsNAPType(type)) return false;

        // if not selected before, it is a candidate.
        if (!selectedBefore.TryGetValue(package.Id, out var types)) return true;

        // if not selected before for the requested nap type it is a candidate.
        return !types.Contains(type);
    }
}