using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation1_2;

public class PreviouslySelectedDatasets
{
    private readonly Dictionary<NAPType, HashSet<Guid>> _organizations;
    private readonly Dictionary<NAPType, HashSet<Guid>> _packages;

    public PreviouslySelectedDatasets()
    {
        _organizations = new Dictionary<NAPType, HashSet<Guid>>();
        _packages = new Dictionary<NAPType, HashSet<Guid>>();
    }

    public void AddOrganization(NAPType type, Organization organization)
    {
        if (!_organizations.TryGetValue(type, out var set))
        {
            set = new HashSet<Guid>();
            _organizations.Add(type, set);
        }

        set.Add(organization.Id);
    }

    public void AddPackage(NAPType type, Package package)
    {
        if (!_packages.TryGetValue(type, out var set))
        {
            set = new HashSet<Guid>();
            _packages.Add(type, set);
        }

        set.Add(package.Id);
    }

    public bool OrganizationWasSelected(NAPType type, Guid id)
    {
        if (!_organizations.TryGetValue(type, out var organizations)) return false;

        return organizations.Contains(id);
    }

    public bool PackageWasSelected(NAPType type, Guid id)
    {
        if (!_packages.TryGetValue(type, out var packages)) return false;

        return packages.Contains(id);
    }
}