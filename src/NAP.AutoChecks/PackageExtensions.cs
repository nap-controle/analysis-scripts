using TransportDataBe.Client.Models;
// ReSharper disable InconsistentNaming

namespace NAP.AutoChecks;

public static class PackageExtensions
{
    public static bool IsMMTIS(this Package package)
    {
        return package.NAP_type != null &&
            package.NAP_type.Any(x => x.ToLowerInvariant() == "mmtis");
    }
    public static bool IsSSTP(this Package package)
    {
        return package.NAP_type != null &&
               package.NAP_type.Any(x => x.ToLowerInvariant() == "sstp");
    }
    public static bool IsRTTI(this Package package)
    {
        return package.NAP_type != null &&
               package.NAP_type.Any(x => x.ToLowerInvariant() == "rtti");
    }
    public static bool IsSRTI(this Package package)
    {
        return package.NAP_type != null &&
               package.NAP_type.Any(x => x.ToLowerInvariant() == "srti");
    }
}