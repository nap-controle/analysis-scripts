using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;
// ReSharper disable InconsistentNaming

namespace NAP.AutoChecks;

public static class PackageExtensions
{
    extension(Package package)
    {
        public bool IsMMTIS()
        {
            return package.NAP_type != null &&
                   package.NAP_type.Any(x => x.ToLowerInvariant() == "mmtis");
        }

        public bool IsSSTP()
        {
            return package.NAP_type != null &&
                   package.NAP_type.Any(x => x.ToLowerInvariant() == "sstp");
        }

        public bool IsRTTI()
        {
            return package.NAP_type != null &&
                   package.NAP_type.Any(x => x.ToLowerInvariant() == "rtti");
        }

        public bool IsSRTI()
        {
            return package.NAP_type != null &&
                   package.NAP_type.Any(x => x.ToLowerInvariant() == "srti");
        }

        public bool IsNapType(NAPType napType)
        {
            switch (napType)
            {
                case NAPType.MMTIS:
                    return package.IsMMTIS();
                case NAPType.SSTP:
                    return package.IsSSTP();
                case NAPType.SRTI:
                    return package.IsSRTI();
                case NAPType.RTTI:
                    return package.IsRTTI();
            }

            throw new Exception();
        }
    }
}