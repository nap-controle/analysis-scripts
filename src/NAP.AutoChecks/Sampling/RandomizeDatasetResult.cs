using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;
// ReSharper disable InconsistentNaming

namespace NAP.AutoChecks.Sampling;

public class RandomizeDatasetResult
{
    public RandomizeDatasetResult(Stakeholder stakeholder, Organization organization, Package package, bool hasRTTIPackage, bool hasSRTIPackage, bool hasSSTPPackage, bool hasMMTISPackage)
    {
        this.PackageId = package.Id;
        this.PackageName = package.Name ?? string.Empty;
        this.StakeholderMMTIS = stakeholder.IsMMTIS;
        this.StakeholderRTTI = stakeholder.IsRTTI;
        this.StakeholderSRTI = stakeholder.IsSRTI;
        this.StakeholderSSTP = stakeholder.IsSSTP;
        this.HasRTTIPackage = hasRTTIPackage;
        this.HasSRTIPackage = hasSRTIPackage;
        this.HasSSTPPackage = hasSSTPPackage;
        this.HasMMTISPackage = hasMMTISPackage;
        this.PackageIsMMTIS = package.NAP_type?.Any(x => x.ToLowerInvariant() == "mmtis");
        this.PackageIsSRTI = package.NAP_type?.Any(x => x.ToLowerInvariant() == "srti");
        this.PackageIsSSTP = package.NAP_type?.Any(x => x.ToLowerInvariant() == "sstp");
        this.PackageIsRTTI = package.NAP_type?.Any(x => x.ToLowerInvariant() == "rtti");
        this.MMTIStype = stakeholder.MMTISType;
        this.OrganizationId = organization.Id;
        this.OrganizationName = organization.Name;
    }

    public string OrganizationName { get; set; }
    
    public Guid OrganizationId { get; set; }
    
    public Guid PackageId { get; set; }

    public string PackageName { get; set; }
    
    public bool HasMMTISPackage { get; set; }
    
    public bool HasRTTIPackage { get; set; }
    
    public bool HasSRTIPackage { get; set; }
    
    public bool HasSSTPPackage { get; set; }

    public bool? PackageIsRTTI { get; set; }

    public bool? PackageIsSSTP { get; set; }

    public bool? PackageIsSRTI { get; set; }

    public bool? PackageIsMMTIS { get; set; }

    public bool StakeholderSSTP { get; set; }

    public bool StakeholderSRTI { get; set; }

    public bool StakeholderRTTI { get; set; }

    public bool StakeholderMMTIS { get; set; }
    
    public MMTISType? StakeholderMMTIStype { get; set; }
}