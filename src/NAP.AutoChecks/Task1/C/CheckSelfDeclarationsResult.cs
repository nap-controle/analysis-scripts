using System.Diagnostics;
using NAP.AutoChecks.Domain;
using NAP.AutoChecks.Evaluation2_1;
using TransportDataBe.Client.Models;
namespace NAP.AutoChecks.Task1.C;

[DebuggerDisplay("{this.Id }[{this.Name}] [{this.OrganizationId}] {this.MMTIS} {this.SSTP} {this.RTTI} {this.SRTI}")]
public class CheckSelfDeclarationsResult
{
    public CheckSelfDeclarationsResult(Stakeholder stakeholder, Organization organization)
    {
        this.Id = stakeholder.Id;
        this.Name = stakeholder.Name ?? "";
        this.OrganizationId = stakeholder.OrganizationId ?? "";

        this.MMTIS = organization.HasMMTISDeclaration();
        this.StakeholderIsMMTIS = stakeholder.IsMMTIS;
        this.RTTI = organization.HasRTTIDeclaration();
        this.StakeholderIsRTTI = stakeholder.IsRTTI;
        this.SRTI = organization.HasSRTIDeclaration();
        this.StakeholderIsSRTI = stakeholder.IsSRTI;
        this.SSTP = organization.HasSSTPDeclaration();
        this.StakeholderIsSSTP = stakeholder.IsSSTP;
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string OrganizationId { get; set; }

    public bool MMTIS { get; set; }
    public bool StakeholderIsMMTIS { get; set; }
    public bool RTTI { get; set; }
    public bool StakeholderIsRTTI { get; set; }

    public bool SRTI { get; set; }
    public bool StakeholderIsSRTI { get; set; }


    public bool SSTP { get; set; }
    public bool StakeholderIsSSTP { get; set; }
}