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
        this.RTTI = organization.HasRTTIDeclaration();
        this.SRTI = organization.HasSRTIDeclaration();
        this.SSTP = organization.HasSSTPDeclaration();
    }
    
    public string Id { get; set; }

    public string Name { get; set; }

    public string OrganizationId { get; set; }

    public bool MMTIS { get; set; }
    public bool RTTI { get; set; }
    
    public bool SRTI { get; set; }
    
    public bool SSTP { get; set; }
}