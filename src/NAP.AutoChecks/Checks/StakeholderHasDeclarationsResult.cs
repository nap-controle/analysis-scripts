using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Checks;

public class StakeholderHasDeclarationsResult
{
    /// <summary>
    /// Creates a new result.
    /// </summary>
    /// <param name="stakeholder"></param>
    /// <param name="error"></param>
    /// <param name="organization"></param>
    /// <param name="packages"></param>
    public StakeholderHasDeclarationsResult(Stakeholder stakeholder, string error, Organization organization, IEnumerable<Package> packages)
    {
        this.Id = stakeholder.Id;
        this.Name = stakeholder.Name;
        this.OrganizationId = stakeholder.OrganizationId;
        this.Error = error;
        this.StakeholderMMTIS = stakeholder.IsMMTIS;
        this.StakeholderRTTI = stakeholder.IsRTTI;
        this.StakeholderSRTI = stakeholder.IsSRTI;
        this.StakeholderSSTP = stakeholder.IsSSTP;
        this.HasMMTISPackage = organization.HasMMTISPackage(packages);
        this.HasSRTIPackage = organization.HasSRTIPackage(packages);
        this.HasSSTPPackage = organization.HasSSTPPackage(packages);
        this.HasRTTIPackage = organization.HasRTTIPackage(packages);
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
    public bool StakeholderSRTI { get; set; }

    public bool HasSRTIPackage { get; set; }

    public bool StakeholderRTTI { get; set; }

    public bool HasRTTIPackage { get; set; }

    public bool StakeholderMMTIS { get; set; }

    public bool HasMMTISPackage { get; set; }

}