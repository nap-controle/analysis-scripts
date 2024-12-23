namespace NAP.AutoChecks.Evaluation2_2._2022;

public class SelectedIn2022OrganizationLoader
{
    private readonly SelectedIn2022OrganizationLoaderSettings _loaderSettings;

    public SelectedIn2022OrganizationLoader(SelectedIn2022OrganizationLoaderSettings loaderSettings)
    {
        _loaderSettings = loaderSettings;
    }

    public async Task<IEnumerable<SelectedIn2022Organization>> Get()
    {
        var path = Path.Combine(_loaderSettings.DataPath, "2022", "2.2", "selected_organizations.csv");
        await using var stream =
            File.OpenRead(path);
        return await SelectedIn2022Organization.Load(stream);
    }
}