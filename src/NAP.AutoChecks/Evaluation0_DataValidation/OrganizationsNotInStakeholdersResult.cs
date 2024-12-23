using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation0_DataValidation;

public class OrganizationsNotInStakeholdersResult
{
    public OrganizationsNotInStakeholdersResult(Organization organization)
    {
        this.Id = organization.Id;
        this.Title = organization.Title;
        this.Name = organization.Name;
    }
    
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Name { get; set; }
}