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
}