using Microsoft.Extensions.Logging;
using NAP.AutoChecks.API;
using NAP.AutoChecks.Evaluation2_1;
using NAP.AutoChecks.Evaluation2_2._2022;
using NAP.AutoChecks.Evaluation2_2._2023;

namespace NAP.AutoChecks.Evaluation2_2;

public class RandomizeOrganizationsWithDeclarations
{
    private readonly DataHandler _dataHandler;
    private readonly int _maxMMTIS = 25;
    private readonly int _maxSRTI = 5;
    private readonly int _maxRTTI = 5;
    private readonly int _maxSSTP = 5;
    private readonly ILogger<RandomizeOrganizationsWithDeclarations> _logger;
    private readonly SelectedIn2022OrganizationLoader _selectedLoader2022;
    private readonly SelectedIn2023OrganizationLoader _selectedLoader2023;
    private readonly RandomizeOrganizationsWithDeclarationsSettings _settings;

    public RandomizeOrganizationsWithDeclarations(DataHandler dataHandler, ILogger<RandomizeOrganizationsWithDeclarations> logger, SelectedIn2022OrganizationLoader selectedLoader2022, RandomizeOrganizationsWithDeclarationsSettings settings, SelectedIn2023OrganizationLoader selectedLoader2023)
    {
        _dataHandler = dataHandler;
        _logger = logger;
        _selectedLoader2022 = selectedLoader2022;
        _settings = settings;
        _selectedLoader2023 = selectedLoader2023;
    }

    public async Task Run()
    {
        var stakeholders = await _dataHandler.GetStakeholders();

        var organizations = await _dataHandler.GetOrganizations();
        
        var packages = (await _dataHandler.GetPackages()).ToList();

        var results = new List<RandomizeOrganizationsWithDeclarationsResults>();
        foreach (var stakeholder in stakeholders)
        {
            if (stakeholder.ParsedOrganizationId == null)
            {
                //results.Add(new StakeholderHasPackagesResult(stakeholder, "no_organization_id"));
                continue;
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var organization = organizations.FirstOrDefault(x => x.Id == stakeholder.ParsedOrganizationId);
            if (organization == null)
            {
                continue;
            }

            if (organization.HasMMTISDeclaration() || organization.HasRTTIDeclaration() || organization.HasSRTIDeclaration() || organization.HasSSTPDeclaration())
            {
                results.Add(new RandomizeOrganizationsWithDeclarationsResults(stakeholder, organization, packages, _settings.PreviousSamplingDay));
            }
            else
            {
                _logger.LogWarning("Organization has no declarations submitted: {OrganizationName}", organization.Name);
            }
        }
        
        results.Shuffle();

        // sort previously selected at the bottom.
        var previousSelected2022 = (await _selectedLoader2022.Get()).ToList();
        var previousSelected2023 = (await _selectedLoader2023.Get()).ToList();
        var previouslySelected = previousSelected2022
            .Where(x => x.SelectedSRTI || x.SelectedMMTIS || x.SelectedRTTI || x.SelectedSSTP)
            .Select(x => x.OrganizationId).ToHashSet();
        previouslySelected.UnionWith(previousSelected2023
            .Where(x => x.SelectedSRTI || x.SelectedMMTIS || x.SelectedRTTI || x.SelectedSSTP)
            .Select(x => x.OrganizationId));
        var newResult =new List<RandomizeOrganizationsWithDeclarationsResults>();
        while (results.Count > 0)
        {
            var last = results[^1];
            results.RemoveAt(results.Count - 1);
            if (previouslySelected.Contains(last.OrganizationId))
            {
                last.SelectedBefore = true;
                newResult.Add(last);
                continue;
            }
            newResult.Insert(0, last);
        }
        results = newResult;
        
        var extraBudget = 0;
        while (results.Count(x => x is { SelectedSRTI: true }) < _maxSRTI)
        {
            // select next.
            var next = results
                .FirstOrDefault(x => x is { HasSRTIDeclaration: true, SelectedSRTI: false }
                                     && (!x.SelectedBefore || x.SRTIWasModified));
            if (next == null)
            {
                _logger.LogWarning(
                    "Could select only {Count} of {Max} organizations for SRTI",
                    results.Count(x => x.SelectedSRTI), _maxSRTI);
                extraBudget += _maxSSTP - results.Count(x => x.SelectedSRTI);
                break;
            }
            next.SelectedSRTI = true;
            
            // report selection.
            _logger.LogInformation("Selected {StakeholderName} as #{Count} for SRTI",
                next.Name, results.Count(x => x.SelectedSRTI));
        }

        while (results.Count(x => x is { SelectedRTTI: true }) < _maxRTTI)
        {
            // select next.
            var next = results
                .FirstOrDefault(x => x is { HasRTTIDeclaration: true, SelectedRTTI: false }
                                     && (!x.SelectedBefore || x.RTTIWasModified));
            if (next == null)
            {
                _logger.LogWarning(
                    "Could select only {Count} of {Max} organizations for RTTI",
                    results.Count(x => x.SelectedRTTI), _maxRTTI);
                extraBudget += _maxRTTI - results.Count(x => x.SelectedRTTI);
                break;
            }
            next.SelectedRTTI = true;
            
            // report selection.
            _logger.LogInformation("Selected {StakeholderName} as #{Count} for RTTI",
                next.Name, results.Count(x => x.SelectedRTTI));
        }

        while (results.Count(x => x is { SelectedSSTP: true }) < _maxSSTP)
        {
            // select next.
            var next = results
                .FirstOrDefault(x => x is { HasSSTPDeclaration: true, SelectedSSTP: false} 
                                     && (!x.SelectedBefore || x.SSTPWasModified));
            if (next == null)
            {
                _logger.LogWarning(
                    "Could select only {Count} of {Max} organizations for SSTP",
                    results.Count(x => x.SelectedSSTP), _maxSSTP);
                extraBudget += _maxSSTP - results.Count(x => x.SelectedSSTP);
                break;
            }
            next.SelectedSSTP = true;
            
            // report selection.
            _logger.LogInformation("Selected {StakeholderName} as #{Count} for SSTP",
                next.Name, results.Count(x => x.SelectedSSTP));
        }

        while (results.Count(x => x is { SelectedMMTIS: true }) < _maxMMTIS + extraBudget)
        {
            // select next.
            var next = results
                .FirstOrDefault(x => x is { HasMMTISDeclaration: true, SelectedMMTIS: false } 
                                     && (!x.SelectedBefore || x.MMTISWasModified));
            if (next == null)
            {
                _logger.LogWarning(
                    "Could select only {Count} of {Max} + {Extra} organizations for MMTIS",
                    results.Count(x => x.SelectedMMTIS), _maxMMTIS, extraBudget);
                break;
            }
            next.SelectedMMTIS = true;
            
            // report selection.
            _logger.LogInformation("Selected {StakeholderName} as #{Count} for MMTIS",
                next.Name, results.Count(x => x.SelectedMMTIS));
        }

        await _dataHandler.WriteResultAsync("evaluation_2.2_randomized_organizations_with_declarations.xlsx", results);
    }
}