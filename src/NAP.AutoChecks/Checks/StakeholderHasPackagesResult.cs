using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Checks;

public class StakeholderHasPackagesResult
{
    /// <summary>
    /// Creates a new result.
    /// </summary>
    /// <param name="stakeholder"></param>
    /// <param name="error"></param>
    public StakeholderHasPackagesResult(Stakeholder stakeholder, string error)
    {
        this.Id = stakeholder.Id;
        this.Name = stakeholder.Name;
        this.OrganizationId = stakeholder.OrganizationId;
        this.Error = error;
        this.StakeholderMMTIS = stakeholder.IsMMTIS;
        this.StakeholderRTTI = stakeholder.IsRTTI;
        this.StakeholderSRTI = stakeholder.IsSRTI;
        this.StakeholderSSTP = stakeholder.IsSSTP;
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

    public bool StakeholderSRTI { get; set; }

    public bool StakeholderRTTI { get; set; }

    public bool StakeholderMMTIS { get; set; }
}