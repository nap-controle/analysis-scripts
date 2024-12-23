using TransportDataBe.Client.Models;

namespace NAP.AutoChecks;

public static class OrganizationExtensions
{
    public static bool HasMMTISPackage(this Organization organization, IEnumerable<Package> packages)
    {
        return packages.Where(package => package.Organization.Id == organization.Id)
            .Any(package => package.IsMMTIS());
    }

    public static bool WasModifiedSince(this Organization organization, IEnumerable<Package> packages, 
        DateTime previousSamplingDate, Func<Package, bool> isType)
    {
        var relevantPackages = packages.Where(package => package.Organization.Id == organization.Id && isType(package))
            .ToList();
        foreach (var package in relevantPackages)
        {
            if (package.Metadata_Created.HasValue && package.Metadata_Created.Value >= previousSamplingDate) return true;
            if (package.Metadata_Modified.HasValue && package.Metadata_Modified.Value >= previousSamplingDate) return true;
        }

        return false;
    }
    
    public static bool HasRTTIPackage(this Organization organization, IEnumerable<Package> packages)
    {
        return packages.Where(package => package.Organization.Id == organization.Id)
            .Any(package => package.IsRTTI());
    }
    
    public static bool HasSSTPPackage(this Organization organization, IEnumerable<Package> packages)
    {
        return packages.Where(package => package.Organization.Id == organization.Id)
            .Any(package => package.IsSSTP());
    }
    
    public static bool HasSRTIPackage(this Organization organization, IEnumerable<Package> packages)
    {
        return packages.Where(package => package.Organization.Id == organization.Id)
            .Any(package => package.IsSRTI());
    }
}