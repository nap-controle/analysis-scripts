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
        this.IsMMTIS = package.NAP_type?.Any(x => x.ToLowerInvariant() == "mmtis");
        this.IsSRTI = package.NAP_type?.Any(x => x.ToLowerInvariant() == "srti");
        this.IsSSTP = package.NAP_type?.Any(x => x.ToLowerInvariant() == "sstp");
        this.IsRTTI = package.NAP_type?.Any(x => x.ToLowerInvariant() == "rtti");
        this.MMTIStype = stakeholder.MMTISType;
        this.OrganizationId = organization.Id;
        this.OrganizationName = organization.Name;
    }

    public bool? IsRTTI { get; set; }

    public bool? IsSSTP { get; set; }

    public bool? IsSRTI { get; set; }

    public bool? IsMMTIS { get; set; }

    public bool StakeholderSSTP { get; set; }

    public bool StakeholderSRTI { get; set; }

    public bool StakeholderRTTI { get; set; }

    public bool StakeholderMMTIS { get; set; }

    public string OrganizationName { get; set; }
    
    public Guid OrganizationId { get; set; }
    
    public Guid PackageId { get; set; }

    public string PackageName { get; set; }
    
    public bool HasMMTISPackage { get; set; }
    
    public MMTISType? MMTIStype { get; set; }
    
    public bool HasRTTIPackage { get; set; }
    
    public bool HasSRTIPackage { get; set; }
    
    public bool HasSSTPPackage { get; set; }
}