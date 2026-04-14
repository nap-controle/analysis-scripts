using Microsoft.Extensions.Logging;
using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;
using NAP.AutoChecks.Evaluation1_2;

namespace NAP.AutoChecks.Task3._2025;

public class RandomSelection
{
    private readonly DataHandler _dataHandler;
    private readonly ILogger<RandomSelection> _logger;

    public RandomSelection(DataHandler dataHandler, ILogger<RandomSelection> logger)
    {
        _dataHandler = dataHandler;
        _logger = logger;
    }

    public async Task<IEnumerable<RandomSelectionResult>> Run()
    {
        var packages = (await _dataHandler.GetPackages()).ToList();

        var selectedBefore = new Dictionary<Guid, HashSet<NAPType>>();

        var pools = new[]
        {
            RandomSelectionPool.CreateFrom(packages, NAPType.MMTIS, 15, new HashSet<Guid>()),
            RandomSelectionPool.CreateFrom(packages, NAPType.RTTI, 5, new HashSet<Guid>()),
            RandomSelectionPool.CreateFrom(packages, NAPType.SRTI, 5, new HashSet<Guid>()),
            RandomSelectionPool.CreateFrom(packages, NAPType.SSTP, 5, new HashSet<Guid>()),
        };

        // move extra capacity to the largest pool.
        var biggestPool = pools.OrderByDescending(x => x.Quota).First();
        foreach (var pool in pools)
        {
            if (pool.NAPType == biggestPool.NAPType) continue;
            if (pool.IsFull) continue;

            _logger.LogInformation(
                "There are {Free} out of {Quota} free for {NAPType}, allocating those to {MMTISType}",
                pool.Free, pool.Quota, pool.NAPType, biggestPool.NAPType);
            biggestPool.IncreaseQuota(pool.Free);
        }

        // move duplicate datasets to the biggest pool.
        var selectedCount = new Dictionary<Guid, int>();
        foreach (var pool in pools)
        {
            foreach (var selected in pool.GetSelected())
            {
                if (!selectedCount.TryGetValue(selected.Id, out _)) selectedCount[selected.Id] = 0;

                selectedCount[selected.Id] += 1;
            }
        }
        foreach (var (id, count) in selectedCount)
        {
            if (count <= 1) continue;

            var package = packages.First(x => x.Id == id);

            _logger.LogInformation(
                "Package {PackageName} is selected {Count} times, allocating {Extra} to {MMTISType}",
                package.Name, count, count - 1, biggestPool.NAPType);
            biggestPool.IncreaseQuota(count - 1);
        }

        var selection = new List<RandomSelectionResult>();
        foreach (var pool in pools)
        {
            selection.AddRange(pool.GetSelected().Select(x => new RandomSelectionResult(x, pool.NAPType)));
        }
        return selection;
    }
}