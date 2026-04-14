namespace NAP.AutoChecks;

public static class Constants
{
    public static HashSet<string> OrganizationsBlacklist { get; set; } = [
        "geo-solutions",
        "organization_test240807",
        "partago",
        "fc16f6dd-4007-4651-b3de-a235f8b058d3",
        "belgian-its-steering-committee-comite-belge-de-pilotage-sti-belgisch-its-stuurgroep-belgischer-its-lenkungsausschuss"
    ];
}