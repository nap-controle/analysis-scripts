namespace NAP.AutoChecks.Evaluation1_2._2022;

public class PreviouslySelectedDatasetLoader
{
    private readonly PreviouslySelectedDatasetsSettings _settings;

    public PreviouslySelectedDatasetLoader(PreviouslySelectedDatasetsSettings settings)
    {
        _settings = settings;
    }

    public async Task<IEnumerable<PreviouslySelectedDataset>> Get()
    {
        var path = Path.Combine(_settings.DataPath, "2022", "1.2", "selected_datasets.csv");
        await using var stream =
            File.OpenRead(path);
        return await PreviouslySelectedDataset.Load(stream);
    }
}