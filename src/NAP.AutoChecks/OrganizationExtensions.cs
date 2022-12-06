using TransportDataBe.Client.Models;

namespace NAP.AutoChecks;

public static class OrganizationExtensions
{
    public static bool HasMMTISPackage(this Organization organization, IEnumerable<Package> packages)
    {
        return packages.Where(package => package.Organization.Id == organization.Id)
            .Any(package => package.IsMMTIS());
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