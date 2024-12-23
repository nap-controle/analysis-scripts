using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Evaluation0_DataValidation;

public class StakeholderWithoutNAPType : Stakeholder
{
    public StakeholderWithoutNAPType(Stakeholder stakeholder, string? remarks = null)
    {
        this.Id = stakeholder.Id;
        this.Name = stakeholder.Name;
        this.OrganizationId = stakeholder.OrganizationId;
        this.IsRTTI = stakeholder.IsRTTI;
        this.IsSRTI = stakeholder.IsSRTI;
        this.IsMMTIS = stakeholder.IsMMTIS;
        this.IsSSTP = stakeholder.IsSSTP;
        this.MMTISType = stakeholder.MMTISType;
        this.Remarks = remarks ?? string.Empty;
    }

    public bool HasMMTISPackage { get; set; } = false;

    public bool HasRTTIPackage { get; set; } = false;
    
    public bool HasSRTIPackage { get; set; } = false;

    public bool HasSSTPPackage { get; set; } = false;

    public string Remarks { get; set; } = string.Empty;
}