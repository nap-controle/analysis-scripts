using NAP.AutoChecks.Task3._2025;

namespace NAP.AutoChecks.Task3;

public class Task3
{
    private readonly RandomSelection _randomSelection;

    public Task3(RandomSelection randomSelection)
    {
        _randomSelection = randomSelection;
    }

    public async Task Run()
    {
        await _randomSelection.Run();
    }
}