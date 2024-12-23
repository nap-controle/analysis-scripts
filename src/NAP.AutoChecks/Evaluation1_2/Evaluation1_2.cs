namespace NAP.AutoChecks.Evaluation1_2;

public class Evaluation1_2
{
    private readonly StratifiedSampling _stratifiedSampling;

    public Evaluation1_2(StratifiedSampling stratifiedSampling)
    {
        _stratifiedSampling = stratifiedSampling;
    }

    public async Task Run()
    {
        // run stratified sampling to select datasets.
        await _stratifiedSampling.Run();
    }
}