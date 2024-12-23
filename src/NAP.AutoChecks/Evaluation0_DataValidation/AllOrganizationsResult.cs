using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation0_DataValidation;

public class AllOrganizationsResult
{
    public AllOrganizationsResult(Organization organization, Stakeholder? stakeholder)
    {
        this.OrganizationId = organization.Id;
        this.Name = organization.Name;
        this.StakeholderId = stakeholder?.Id;
        this.StakeholderName = stakeholder?.Name;
        this.StakeholderIsRTTI =stakeholder?.IsRTTI;
        this.StakeholderIsSRTI = stakeholder?.IsSRTI;
        this.StakeholderIsSSTP = stakeholder?.IsSSTP;
        this.StakeholderIsMMTIS = stakeholder?.IsMMTIS;
    }
    
    public Guid OrganizationId { get; set; }

    public string Name { get; set; }
    
    /// <summary>
    /// The id.
    /// </summary>
    public string? StakeholderId { get; set; }

    /// <summary>
    /// The name.
    /// </summary>
    public string? StakeholderName { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool? StakeholderIsMMTIS { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool? StakeholderIsRTTI { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool? StakeholderIsSRTI { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool? StakeholderIsSSTP { get; set; }
}