using NAP.AutoChecks.API;

namespace NAP.AutoChecks.Evaluation0_DataValidation;

public class StakeholderWithoutNAPTypeCheck
{
    private readonly DataHandler _dataHandler;

    public StakeholderWithoutNAPTypeCheck(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }
    
    public async Task Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders();
        var organizations = await _dataHandler.GetOrganizations();
        var packages = await _dataHandler.GetPackages();

        var results = new List<StakeholderWithoutNAPType>();
        foreach (var stakeholder in stakeholders.Where(x =>
                     x is { IsSSTP: false, IsRTTI: false, IsSRTI: false, IsMMTIS: false }))
        {
            if (stakeholder.ParsedOrganizationId == null)
            {
                results.Add(new StakeholderWithoutNAPType(stakeholder, "No organization matched via API, could not suggest NAP types"));
                continue;
            }
            
            // ReSharper disable once PossibleMultipleEnumeration
            var organization = organizations.FirstOrDefault(x => x.Id == stakeholder.ParsedOrganizationId);
            if (organization == null) 
            {
                results.Add(new StakeholderWithoutNAPType(stakeholder, "No organization matched via API, could not suggest NAP types"));
                continue;
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var orgPackages = packages.Where(x => x.Organization.Id == organization.Id)
                .ToList();
            if (orgPackages.Count == 0)
            {
                results.Add(new StakeholderWithoutNAPType(stakeholder, "No packages, could not suggest NAP types"));
            }
            else
            {
                var result = new StakeholderWithoutNAPType(stakeholder);
                foreach (var orgPackage in orgPackages)
                {
                    result.HasMMTISPackage = result.HasMMTISPackage || orgPackage.IsMMTIS();
                    result.HasRTTIPackage = result.HasRTTIPackage || orgPackage.IsRTTI();
                    result.HasSSTPPackage = result.HasSSTPPackage || orgPackage.IsSSTP();
                    result.HasSRTIPackage = result.HasSRTIPackage || orgPackage.IsSRTI(); 
                }
                results.Add(result);
            }
        }

        await _dataHandler.WriteResultAsync("evaluation_0_stakeholders_without_nap_type.xlsx", results);
    }
}