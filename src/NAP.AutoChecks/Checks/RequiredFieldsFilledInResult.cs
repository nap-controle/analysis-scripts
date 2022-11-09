using NAP.AutoChecks.Domain;
using TransportDataBe.Client;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Checks;

public class RequiredFieldsFilledInResult
{
    private readonly Client _client;

    /// <summary>
    /// Creates a new result.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="stakeholder"></param>
    /// <param name="package"></param>
    /// <param name="error"></param>
    /// <param name="message"></param>
    public RequiredFieldsFilledInResult(Client client, Stakeholder stakeholder, Package package, string error, string message)
    {
        _client = client;
        
        this.Id = stakeholder.Id;
        this.Name = stakeholder.Name;
        this.OrganizationId = stakeholder.OrganizationId;
        this.Error = error;
        this.ErrorMessage = message;
        _client = client;
        this.PackageId = package.Id;
        this.PackageName = package.Name ?? string.Empty;
    }

    /// <summary>
    /// Creates a new result.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="stakeholder"></param>
    /// <param name="package"></param>
    /// <param name="resource"></param>
    /// <param name="error"></param>
    /// <param name="message"></param>
    public RequiredFieldsFilledInResult(Client client, Stakeholder stakeholder, Package package, Resource resource, string error, string message)
    {
        _client = client;
        
        this.Id = stakeholder.Id;
        this.Name = stakeholder.Name;
        this.OrganizationId = stakeholder.OrganizationId;
        this.Error = error;
        this.ErrorMessage = message;
        _client = client;
        this.PackageId = package.Id;
        this.PackageName = package.Name ?? string.Empty;
        this.ResourceId = resource?.Id;
        this.ResourceName = resource?.Name;
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
    /// The package id.
    /// </summary>
    public Guid PackageId { get; set; }

    /// <summary>
    /// The package url.
    /// </summary>
    public string PackageUrl => _client.GetPackageUrl(this.PackageName);
    
    /// <summary>
    /// The package name.
    /// </summary>
    public string PackageName { get; set; }
    
    /// <summary>
    /// The resource id.
    /// </summary>
    public Guid? ResourceId { get; set; }
    
    /// <summary>
    /// The resource name.
    /// </summary>
    public string? ResourceName { get; set; }


    /// <summary>
    /// The error.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// The message.
    /// </summary>
    public string ErrorMessage { get; set; }
}