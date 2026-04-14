namespace TransportDataBe.Client.Models;

public class Package
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string Title { get; set; }

    public string[]? Language { get; set; }

    public string[]? countries_covered { get; set; }

    public string? mobility_theme { get; set; }

    public bool Private { get; set; }

    public DateTime? Metadata_Created { get; set; }

    public DateTime? Metadata_Modified { get; set; }

    public DateTime? Temporal_Start { get; set; }

    public string[]? Regions_Covered { get; set; }

    public Guid? Owner_Org { get; set; }

    public string? contact_point_name { get; set; }

    public string? contact_point_email { get; set; }

    public string? publisher_firstname { get; set; }

    public string? publisher_surname { get; set; }

    public string? Frequency { get; set; }

    public Resource[]? Resources { get; set; }

    public PackageOrganization Organization { get; set; }

    public TranslatedText? Notes_Translated { get; set; }

    public string? Fluent_Tags { get; set; }

    public string? Cont_Res { get; set; }

    public string[]? NAP_type { get; set; }
}