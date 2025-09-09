using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Task3._2025;

public class RandomSelectionResult
{
    public RandomSelectionResult(Package package, NAPType type)
    {
        this.Id = package.Id;
        this.Name = package.Name ?? "";
        this.Type = type;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public NAPType Type { get; set; }
}