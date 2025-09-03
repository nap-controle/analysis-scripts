using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Task3._2025;

public class RandomSelectionPool
{
    private readonly IEnumerable<Package> _candidates;
    private readonly HashSet<Guid> _selectedBefore;
    private readonly HashSet<Guid> _selected = [];
    private int _extra = 0;

    public RandomSelectionPool(NAPType type, IEnumerable<Package> candidates, int quota, HashSet<Guid> selectedBefore)
    {
        NAPType = type;
        _candidates = candidates;
        Quota = quota;
        _selectedBefore = selectedBefore;

        this.Select();
    }

    public static RandomSelectionPool CreateFrom(IEnumerable<Package> packages, NAPType type, int defaultQuota, 
        HashSet<Guid> selectedBefore)
    {
        return new RandomSelectionPool(type, packages.Where(x => x.IsNAPType(type)), 
            defaultQuota, selectedBefore);
    }
    
    public NAPType NAPType { get; }
    
    public int Quota { get; }

    public int Free => Quota + _extra - _selected.Count;

    public bool IsFull => this.Free == 0;

    public bool IncreaseQuota(int extra)
    {
        _extra += extra;

        return this.Select();
    }

    public bool IsSelected(Guid packageId)
    {
        return _selected.Contains(packageId);
    }

    public IEnumerable<Package> GetSelected()
    {
        return _selected.Select(guid => _candidates.First(x => x.Id == guid));
    }
    
    private bool Select()
    {
        var selected = _candidates
            .Where(x => !this._selected.Contains(x.Id) && !this._selectedBefore.Contains(x.Id))
            .SelectRandom(Quota + _extra - _selected.Count);
        var success = false;
        foreach (var package in selected)
        {
            success = true;
            this._selected.Add(package.Id);
        }
        return success;
    }
}