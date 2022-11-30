using NAP.AutoChecks.Checks;
using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;
// ReSharper disable InconsistentNaming

namespace NAP.AutoChecks.Sampling;

public class RandomizeDatasetResult
{
    public RandomizeDatasetResult(Stakeholder stakeholder, Organization organization, Package package, bool orgHasRttiPackage, bool orgHasSrtiPackage, bool orgHasSstpPackage, bool orgHasMmtisPackage)
    {
        this.PackageId = package.Id;
        this.PackageName = package.Name ?? string.Empty;
        this.StakeholderMMTIS = stakeholder.IsMMTIS;
        this.StakeholderRTTI = stakeholder.IsRTTI;
        this.StakeholderSRTI = stakeholder.IsSRTI;
        this.StakeholderSSTP = stakeholder.IsSSTP;
        this.OrgHasRTTIPackage = orgHasRttiPackage;
        this.OrgHasSRTIPackage = orgHasSrtiPackage;
        this.OrgHasSSTPPackage = orgHasSstpPackage;
        this.OrgHasMMTISPackage = orgHasMmtisPackage;
        this.OrgHasRTTIDeclaration = organization.HasRTTIDeclaration();
        this.OrgHasSRTIDeclaration = organization.HasSRTIDeclaration();
        this.OrgHasSSTPDeclaration = organization.HasSSTPDeclaration();
        this.OrgHasMMTISDeclaration = organization.HasMMTISDeclaration();
        this.PackageIsMMTIS = package.NAP_type?.Any(x => x.ToLowerInvariant() == "mmtis");
        this.PackageIsSRTI = package.NAP_type?.Any(x => x.ToLowerInvariant() == "srti");
        this.PackageIsSSTP = package.NAP_type?.Any(x => x.ToLowerInvariant() == "sstp");
        this.PackageIsRTTI = package.NAP_type?.Any(x => x.ToLowerInvariant() == "rtti");
        this.StakeholderMMTIStype = stakeholder.MMTISType;
        this.OrganizationId = organization.Id;
        this.OrganizationName = organization.Name;
    }

    public string OrganizationName { get; set; }
    
    public Guid OrganizationId { get; set; }
    
    public Guid PackageId { get; set; }

    public string PackageName { get; set; }
    
    // MMTIS Section.

    public bool? PackageIsMMTIS { get; set; }
    
    public bool OrgHasMMTISPackage { get; set; }
    
    public bool OrgHasMMTISDeclaration { get; set; }

    public bool StakeholderMMTIS { get; set; }
    
    public MMTISType? StakeholderMMTIStype { get; set; }
    
    // RTTI Section.

    public bool? PackageIsRTTI { get; set; }
    
    public bool OrgHasRTTIPackage { get; set; }
    
    public bool OrgHasRTTIDeclaration { get; set; }

    public bool StakeholderRTTI { get; set; }
    
    // SSTP Section.

    public bool? PackageIsSSTP { get; set; }
    
    public bool OrgHasSSTPPackage { get; set; }
    
    public bool OrgHasSSTPDeclaration { get; set; }

    public bool StakeholderSSTP { get; set; }
    
    // SRTI Section.

    public bool? PackageIsSRTI { get; set; }
    
    public bool OrgHasSRTIPackage { get; set; }
    
    public bool OrgHasSRTIDeclaration { get; set; }

    public bool StakeholderSRTI { get; set; }
}