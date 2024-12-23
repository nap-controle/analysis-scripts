using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation2_1;

public class StakeholdersWithDeclarationsResult
{
    public StakeholdersWithDeclarationsResult(Stakeholder stakeholder, Organization organization)
    {
        this.OrgHasRTTIDeclaration = organization.HasRTTIDeclaration();
        this.OrgHasSRTIDeclaration = organization.HasSRTIDeclaration();
        this.OrgHasSSTPDeclaration = organization.HasSSTPDeclaration();
        this.OrgHasMMTISDeclaration = organization.HasMMTISDeclaration();
        this.StakeholderMMTIStype = stakeholder.MMTISType;
        this.OrganizationId = organization.Id;
        this.OrganizationName = organization.Name;
    }
    

    public string OrganizationName { get; set; }
    
    public Guid OrganizationId { get; set; }

    public bool OrgHasMMTISDeclaration { get; set; }
    
    public MMTISType? StakeholderMMTIStype { get; set; }
    
    public bool OrgHasRTTIDeclaration { get; set; }
    
    public bool OrgHasSSTPDeclaration { get; set; }
    
    public bool OrgHasSRTIDeclaration { get; set; }
}