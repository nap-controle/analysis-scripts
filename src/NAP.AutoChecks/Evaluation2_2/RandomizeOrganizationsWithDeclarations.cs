using Microsoft.Extensions.Logging;
using NAP.AutoChecks.API;
using NAP.AutoChecks.Evaluation2_1;
using NAP.AutoChecks.Evaluation2_2._2022;

namespace NAP.AutoChecks.Evaluation2_2;

public class RandomizeOrganizationsWithDeclarations
{
    private readonly DataHandler _dataHandler;
    private readonly int _maxMMTIS = 25;
    private readonly int _maxSRTI = 5;
    private readonly int _maxRTTI = 5;
    private readonly int _maxSSTP = 5;
    private readonly ILogger<RandomizeOrganizationsWithDeclarations> _logger;
    private readonly PreviouslySelectedDatasets2_2Loader _previouslySelectedLoader;
    private readonly RandomizeOrganizationsWithDeclarationsSettings _settings;

    public RandomizeOrganizationsWithDeclarations(DataHandler dataHandler, ILogger<RandomizeOrganizationsWithDeclarations> logger, PreviouslySelectedDatasets2_2Loader previouslySelectedLoader, RandomizeOrganizationsWithDeclarationsSettings settings)
    {
        _dataHandler = dataHandler;
        _logger = logger;
        _previouslySelectedLoader = previouslySelectedLoader;
        _settings = settings;
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
        var previousSelection = (await _previouslySelectedLoader.Get()).ToList();
        var previouslySelected = previousSelection
            .Where(x => x.SelectedSRTI || x.SelectedMMTIS || x.SelectedRTTI || x.SelectedSSTP)
            .Select(x => x.OrganizationId).ToHashSet();
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
        while (results.Count(x => x is { SelectedSRTI: true, SRTIWasModified: true }) < _maxSRTI)
        {
            // select next.
            var next = results.FirstOrDefault(x => x.HasSRTIDeclaration && !x.SelectedSRTI);
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

        while (results.Count(x => x is { SelectedRTTI: true, RTTIWasModified: true }) < _maxRTTI)
        {
            // select next.
            var next = results.FirstOrDefault(x => x.HasRTTIDeclaration && !x.SelectedRTTI);
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

        while (results.Count(x => x is { SelectedSSTP: true, SSTPWasModified: true }) < _maxSSTP)
        {
            // select next.
            var next = results.FirstOrDefault(x => x.HasSSTPDeclaration && !x.SelectedSSTP);
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

        while (results.Count(x => x is { SelectedMMTIS: true, MMTISWasModified: true }) < _maxMMTIS + extraBudget)
        {
            // select next.
            var next = results.FirstOrDefault(x => x.HasMMTISDeclaration && !x.SelectedMMTIS);
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