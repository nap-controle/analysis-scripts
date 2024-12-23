using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation1_2;

public class StratifiedSamplingResult
{
    public StratifiedSamplingResult(Stakeholder stakeholder, Organization organization, Package package)
    {
        this.PackageId = package.Id;
        this.PackageName = package.Name ?? string.Empty;
        this.StakeholderMMTIS = stakeholder.IsMMTIS;
        this.StakeholderRTTI = stakeholder.IsRTTI;
        this.StakeholderSRTI = stakeholder.IsSRTI;
        this.StakeholderSSTP = stakeholder.IsSSTP;
        this.PackageLastModified = package.Metadata_Modified ?? package.Metadata_Created;
        this.PackageIsMMTIS = package.NAP_type?.Any(x => x.ToLowerInvariant() == "mmtis") ?? false;
        this.PackageIsSRTI = package.NAP_type?.Any(x => x.ToLowerInvariant() == "srti") ?? false;
        this.PackageIsSSTP = package.NAP_type?.Any(x => x.ToLowerInvariant() == "sstp") ?? false;
        this.PackageIsRTTI = package.NAP_type?.Any(x => x.ToLowerInvariant() == "rtti") ?? false;
        this.StakeholderMMTIStype = stakeholder.MMTISType;
        this.OrganizationId = organization.Id;
        this.OrganizationName = organization.Name;
    }

    public string OrganizationName { get; set; }
    
    public Guid OrganizationId { get; set; }
    
    public Guid PackageId { get; set; }

    public string PackageName { get; set; }
    
    public DateTime? PackageLastModified { get; set; }
    
    public bool SelectedMMTIS { get; set; }
    
    public bool SelectedSRTI { get; set; }
    
    public bool SelectedRTTI { get; set; }
    
    public bool SelectedSSTP { get; set; }
    
    public string SelectedReason { get; set; }

    // MMTIS Section.

    public bool PackageIsMMTIS { get; set; }
    
    //public bool OrgHasMMTISPackage { get; set; }
    
    //public bool OrgHasMMTISDeclaration { get; set; }

    public bool StakeholderMMTIS { get; set; }
    
    public MMTISType? StakeholderMMTIStype { get; set; }
    
    // RTTI Section.

    public bool PackageIsRTTI { get; set; }
    
    //public bool OrgHasRTTIPackage { get; set; }
    
    //public bool OrgHasRTTIDeclaration { get; set; }

    public bool StakeholderRTTI { get; set; }
    
    // SSTP Section.

    public bool PackageIsSSTP { get; set; }
    
    //public bool OrgHasSSTPPackage { get; set; }
    
    public bool OrgHasSSTPDeclaration { get; set; }

    public bool StakeholderSSTP { get; set; }
    
    // SRTI Section.

    public bool PackageIsSRTI { get; set; }
    
    public bool OrgHasSRTIPackage { get; set; }
    
    //public bool OrgHasSRTIDeclaration { get; set; }

    public bool StakeholderSRTI { get; set; }

    public bool WasModifiedAfter(DateTime day)
    {
        if (this.PackageLastModified == null) throw new Exception("No last modified date");
        
        return this.PackageLastModified > day;
    }

    public bool PackageIsFor(NAPType type)
    {
        return type switch
        {
            NAPType.RTTI => this.PackageIsRTTI,
            NAPType.SRTI => this.PackageIsSRTI,
            NAPType.SSTP => this.PackageIsSSTP,
            NAPType.MMTIS => this.PackageIsMMTIS,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    public bool PackageSelectedFor(NAPType type)
    {
        return type switch
        {
            NAPType.RTTI => this.SelectedRTTI,
            NAPType.SRTI => this.SelectedSRTI,
            NAPType.SSTP => this.SelectedSSTP,
            NAPType.MMTIS => this.SelectedMMTIS,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    public void SetSelectedFor(NAPType napType, string reason)
    {
        switch (napType)
        {
            case NAPType.RTTI:
                this.SelectedRTTI = true;
                break;
            case NAPType.SRTI:
                this.SelectedSRTI = true;
                break;
            case NAPType.SSTP:
                this.SelectedSSTP = true;
                break;
            case NAPType.MMTIS:
                this.SelectedMMTIS = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(napType), napType, null);
        }

        this.SelectedReason = reason;
    }
}