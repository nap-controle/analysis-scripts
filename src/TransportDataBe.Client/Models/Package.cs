namespace TransportDataBe.Client.Models;

public class Package
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }
    
    public string[]? Language { get; set; }
    
    public DateTime? Metadata_Created { get; set; }
    
    public DateTime? Metadata_Modified { get; set; }
    
    public DateTime? Temporal_Start { get; set; }
    
    public string[]? Regions_Covered { get; set; }
    
    public string? Theme { get; set; }
    
    public string? Publisher_Name { get; set; }
    
    public string? Publisher_Email { get; set; }
    
    public Guid? Owner_Org { get; set; }
    
    public string? Contract_License { get; set; }
    
    public string? License_Id { get; set; }
    
    public string? Frequency { get; set; }
    
    public Resource[]? Resources { get; set; }

    public PackageOrganization Organization { get; set; }    
    
    public NotesTranslated? Notes_Translated { get; set; }
    
    public string? Fluent_Tags { get; set; }
    
    public string? Cont_Res { get; set; }
}