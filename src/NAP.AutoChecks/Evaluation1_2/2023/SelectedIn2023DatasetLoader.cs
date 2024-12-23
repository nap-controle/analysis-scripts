namespace NAP.AutoChecks.Evaluation1_2._2023;

public class SelectedIn2023DatasetLoader
{
    private readonly SelectedIn2023DatasetsSettings _settings;

    public SelectedIn2023DatasetLoader(SelectedIn2023DatasetsSettings settings)
    {
        _settings = settings;
    }

    public async Task<IEnumerable<SelectedIn2023Dataset>> Get()
    {
        var path = Path.Combine(_settings.DataPath, "2023", "1.2", "selected_datasets.csv");
        await using var stream =
            File.OpenRead(path);
        return await SelectedIn2023Dataset.Load(stream);
    }
}