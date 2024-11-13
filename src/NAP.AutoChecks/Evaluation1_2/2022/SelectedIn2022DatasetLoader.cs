namespace NAP.AutoChecks.Evaluation1_2._2022;

public class SelectedIn2022DatasetLoader
{
    private readonly SelectedIn2022DatasetsSettings _settings;

    public SelectedIn2022DatasetLoader(SelectedIn2022DatasetsSettings settings)
    {
        _settings = settings;
    }

    public async Task<IEnumerable<SelectedIn2022Dataset>> Get()
    {
        var path = Path.Combine(_settings.DataPath, "2022", "1.2", "selected_datasets.csv");
        await using var stream =
            File.OpenRead(path);
        return await SelectedIn2022Dataset.Load(stream);
    }
}