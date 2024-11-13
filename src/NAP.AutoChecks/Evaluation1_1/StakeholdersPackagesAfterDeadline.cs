using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Evaluation1_1;

public class StakeholdersPackagesAfterDeadline
{
    private readonly DataHandler _dataHandler;
    private readonly MMTISDeadlineSettings _mmtisDeadlineSettings;

    public StakeholdersPackagesAfterDeadline(DataHandler dataHandler, MMTISDeadlineSettings mmtisDeadlineSettings)
    {
        _dataHandler = dataHandler;
        _mmtisDeadlineSettings = mmtisDeadlineSettings;
    }

    public async Task Check()
    {
        if (_mmtisDeadlineSettings.Deadline == null) throw new ArgumentNullException(nameof(_mmtisDeadlineSettings.Deadline));
        
        var stakeholders = await _dataHandler.GetStakeholders();
        var organizations = await _dataHandler.GetOrganizations();
        var packages = await _dataHandler.GetPackages();

        var results = new List<StakeholdersPackagesAfterDeadlineResult>();
        foreach (var stakeholder in stakeholders)
        {
            if (stakeholder.ParsedOrganizationId == null) continue;
            
            // ReSharper disable once PossibleMultipleEnumeration
            var organization = organizations.FirstOrDefault(x => x.Id == stakeholder.ParsedOrganizationId);
            if (organization == null) continue;

            // ReSharper disable once PossibleMultipleEnumeration
            var packagesForOrg = packages.Where(x => x.Organization.Id == organization.Id);
            foreach (var package in packagesForOrg)
            {
                if (!package.IsMMTIS()) continue;
                
                if (package.Metadata_Created == null) continue;
                if (package.Metadata_Created.Value.Date <= _mmtisDeadlineSettings.Deadline.Value.Date) continue;
                
                results.Add(new StakeholdersPackagesAfterDeadlineResult()
                {
                    OrganizationId = organization.Id,
                    OrganizationName = organization.Name,
                    PackageId = package.Id,
                    PackageName = package.Name ?? string.Empty,
                    Metadata_Created = package.Metadata_Created.Value,
                });
            }
        }

        await _dataHandler.WriteResultAsync("evaluation_1.1_MMTIS_packages_after_mmtis_deadline.xlsx", results);
    }
}