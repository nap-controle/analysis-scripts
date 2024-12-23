namespace NAP.AutoChecks.Evaluation2_2._2023;

public class SelectedIn2023OrganizationLoader
{
    private readonly SelectedIn2023OrganizationLoaderSettings _loaderSettings;

    public SelectedIn2023OrganizationLoader(SelectedIn2023OrganizationLoaderSettings loaderSettings)
    {
        _loaderSettings = loaderSettings;
    }

    public async Task<IEnumerable<SelectedIn2023Organization>> Get()
    {
        var path = Path.Combine(_loaderSettings.DataPath, "2023", "2.2", "selected_organizations.csv");
        await using var stream =
            File.OpenRead(path);
        return await SelectedIn2023Organization.Load(stream);
    }
}