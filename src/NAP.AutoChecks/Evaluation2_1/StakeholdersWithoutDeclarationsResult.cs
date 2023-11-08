using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation2_1;

public class StakeholdersWithoutDeclarationsResult
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stakeholder"></param>
    /// <param name="organization"></param>
    /// <param name="packages"></param>
    /// <param name="error"></param>
    /// <param name="hasMMTISDeclaration"></param>
    /// <param name="hasRTTIDeclaration"></param>
    /// <param name="hasSSTPDeclaration"></param>
    /// <param name="hasSsrtiDeclaration"></param>
    public StakeholdersWithoutDeclarationsResult(Stakeholder stakeholder, Organization organization, IEnumerable<Package> packages,
        string error, bool hasMMTISDeclaration, bool hasRTTIDeclaration, bool hasSSTPDeclaration, bool hasSsrtiDeclaration)
    {
        this.Id = stakeholder.Id;
        this.Name = stakeholder.Name;
        this.OrganizationId = stakeholder.OrganizationId;
        this.StakeholderMMTIS = stakeholder.IsMMTIS;
        this.StakeholderRTTI = stakeholder.IsRTTI;
        this.StakeholderSRTI = stakeholder.IsSRTI;
        this.StakeholderSSTP = stakeholder.IsSSTP;
        this.HasMMTISPackage = organization.HasMMTISPackage(packages);
        this.HasSRTIPackage = organization.HasSRTIPackage(packages);
        this.HasSSTPPackage = organization.HasSSTPPackage(packages);
        this.HasRTTIPackage = organization.HasRTTIPackage(packages);
        Error = error;
        this.HasMMTISDeclaration = hasMMTISDeclaration;
        this.HasRTTIDeclaration = hasRTTIDeclaration;
        this.HasSSTPDeclaration = hasSSTPDeclaration;
        this.HasSSRTIDeclaration = hasSsrtiDeclaration;
    }

    /// <summary>
    /// The id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The organization id, if any.
    /// </summary>
    public string OrganizationId { get; set; }

    /// <summary>
    /// The message.
    /// </summary>
    public string Error { get; set; }

    public bool StakeholderSSTP { get; set; }

    public bool HasSSTPPackage { get; set; }
    public bool HasSSTPDeclaration { get; }
    public bool StakeholderSRTI { get; set; }

    public bool HasSRTIPackage { get; set; }
    public bool HasSSRTIDeclaration { get; }

    public bool StakeholderRTTI { get; set; }

    public bool HasRTTIPackage { get; set; }
    public bool HasRTTIDeclaration { get; }

    public bool StakeholderMMTIS { get; set; }

    public bool HasMMTISPackage { get; set; }
    public bool HasMMTISDeclaration { get; }

}