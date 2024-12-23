using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Evaluation1_2;

internal static class StratifiedSamplingResultExtensions
{
    public static bool HasPackageSelected(this IEnumerable<StratifiedSamplingResult> results, NAPType napType,
        Guid organizationId)
    {
        return results.Any(x => x.PackageSelectedFor(napType) &&
                                  x.OrganizationId ==
                                  organizationId);
    }
    
    public static bool Has2PackagesSelected(this IEnumerable<StratifiedSamplingResult> results, NAPType napType,
        Guid organizationId)
    {
        return results.Count(x => x.PackageSelectedFor(napType) &&
                                  x.OrganizationId ==
                                  organizationId) >= 2;
    }
}