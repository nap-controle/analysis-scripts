namespace NAP.AutoChecks.Evaluation2_2._2022;

public class PreviouslySelectedDatasets2_2Loader
{
    private readonly PreviouslySelectedDatasets2_2LoaderSettings _loaderSettings;

    public PreviouslySelectedDatasets2_2Loader(PreviouslySelectedDatasets2_2LoaderSettings loaderSettings)
    {
        _loaderSettings = loaderSettings;
    }

    public async Task<IEnumerable<PreviouslySelectedDataset2_2>> Get()
    {
        var path = Path.Combine(_loaderSettings.DataPath, "2022", "2.2", "selected_organizations.csv");
        await using var stream =
            File.OpenRead(path);
        return await PreviouslySelectedDataset2_2.Load(stream);
    }
}