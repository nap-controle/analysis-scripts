using System.Text.Json;
using NAP.AutoChecks.Domain;
using TransportDataBe.Client;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.API;

public class DataHandler
{
    private readonly Client _client;
    private readonly DataHandlerSettings _dataHandlerSettings;
    private readonly string _todayPath;
    private readonly string _dataPath;

    public DataHandler(Client client, DataHandlerSettings dataHandlerSettings)
    {
        _client = client;
        _dataHandlerSettings = dataHandlerSettings;

        _dataPath = _dataHandlerSettings.DataPath ?? throw new Exception("Data path not set");
        _todayPath = Path.Combine(_dataHandlerSettings.DataPath,
            FormattableString.Invariant($"{DateTime.Today:yyyy-MM-dd}"));
        if (!Directory.Exists(_todayPath))
        {
            Directory.CreateDirectory(_todayPath);
        }
    }

    public async Task<IEnumerable<Stakeholder>> GetStakeholders()
    {
        await using var stream =
            File.OpenRead(Path.Combine(_dataPath, "stakeholders", "organisations.csv"));
        return await Stakeholder.LoadFromCsv(stream);
    }

    public async Task<IEnumerable<Organization>> GetOrganizations()
    {
        var organizationsTodayFile = Path.Combine(_todayPath, "organizations.json");
        var organizationIds = await TryRead<Response<string[]>>(organizationsTodayFile);
        if (organizationIds == null)
        {
            organizationIds = await _client.GetOrganizationList();
            await Write(organizationsTodayFile, organizationIds);
        }

        var organizations = new List<Organization>();
        var organizationsTodayPath = Path.Combine(_todayPath, "organizations");
        if (!Directory.Exists(organizationsTodayPath)) Directory.CreateDirectory(organizationsTodayPath);
        foreach (var organizationId in organizationIds.Result)
        {
            var organizationTodayFile = Path.Combine(_todayPath, "organizations", $"{organizationId}.json");
            var organization = await TryRead<Response<Organization>>(organizationTodayFile);
            if (organization == null)
            {
                organization = await _client.GetOrganization(organizationId);
                await Write(organizationTodayFile, organization);
            }

            organizations.Add(organization.Result);
        }

        return organizations;
    }

    public async Task<IEnumerable<Package>> GetPackages()
    {
        var packagesTodayFile = Path.Combine(_todayPath, "packages.json");
        var packageIds = await TryRead<Response<string[]>>(packagesTodayFile);
        if (packageIds == null)
        {
            packageIds = await _client.GetPackageList();
            await Write(packagesTodayFile, packageIds);
        }

        var packages = new List<Package>();
        var packagesTodayPath = Path.Combine(_todayPath, "packages");
        if (!Directory.Exists(packagesTodayPath)) Directory.CreateDirectory(packagesTodayPath);
        foreach (var packageId in packageIds.Result)
        {
            var packageTodayFile = Path.Combine(_todayPath, "packages", $"{packageId}.json");
            var package = await TryRead<Response<Package>>(packageTodayFile);
            if (package == null)
            {
                package = await _client.GetPackage(packageId);
                await Write(packageTodayFile, package);
            }

            packages.Add(package.Result);
        }

        return packages;
    }

    public async Task WriteResultAsync<T>(string file, IEnumerable<T> items)
    {
        var fileAtData = Path.Combine(_todayPath, file);
        await Csv.WriteAsync(fileAtData, items);
    }

    private static async Task<T?> TryRead<T>(string file)
    {
        if (!File.Exists(file)) return default;
        await using var stream = File.OpenRead(file);
        return await JsonSerializer.DeserializeAsync<T>(stream);
    }

    private static async Task Write<T>(string file, T data)
    {
        await using var stream = File.Open(file, FileMode.Create);
        await JsonSerializer.SerializeAsync(stream, data);
    }
}