using System.Text.Json;
using Microsoft.Extensions.Logging;
using NAP.AutoChecks.API.Stakeholders._2023;
using NAP.AutoChecks.Domain;
using TransportDataBe.Client;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.API;

public class DataHandler
{
    private readonly Client _client;
    private readonly string _todayPath;
    private readonly string _latestPath;
    private readonly string _dataPath;
    private readonly ILogger<DataHandler> _logger;
    private readonly StakeholderLoader _stakeholderLoader;

    public DataHandler(Client client, DataHandlerSettings dataHandlerSettings, ILogger<DataHandler> logger, StakeholderLoader stakeholderLoader)
    {
        _client = client;
        _logger = logger;
        _stakeholderLoader = stakeholderLoader;

        _dataPath = dataHandlerSettings.DataPath ?? throw new Exception("Data path not set");
        _todayPath = Path.Combine(dataHandlerSettings.DataPath,
            FormattableString.Invariant($"{DateTime.Today:yyyy-MM-dd}"));
        if (!Directory.Exists(_todayPath)) Directory.CreateDirectory(_todayPath);
        _latestPath = Path.Combine(dataHandlerSettings.DataPath, "latest");
        if (!Directory.Exists(_latestPath)) Directory.CreateDirectory(_latestPath);
    }

    internal Client GetClient() => _client;

    public IEnumerable<string> GetPossibleLanguages()
    {
        return new[]
        {
            "http://publications.europa.eu/resource/authority/language/FRA",
            "http://publications.europa.eu/resource/authority/language/ENG",
            "http://publications.europa.eu/resource/authority/language/NLD",
            "http://publications.europa.eu/resource/authority/language/DEU"
        };
    }

    public IEnumerable<string> GetPossibleContRes()
    {
        return new[] { "Data set", "Service" };
    }

    public IEnumerable<(string value, bool hasLicense)> GetPossibleContractLicenses()
    {
        return new (string value, bool hasLicense)[] { ("conotfree", true),("cofree", true), ("lifree", true), ("linotfree", true), ("nolinoco", false), ("notrelevant", false) };
    }

    public IEnumerable<string> GetPossibleFormats()
    {
        return new[] { "XML", "JSON", "CSV", "ASN.1 encoding rules", "Protocol buffers", "Other" };
    }
    
    /// <summary>
    /// • DATEX II
    /// • OCIT-C
    /// • DATEX II Light
    /// • NeTEx (CEN/TS 16614)
    /// • SIRI (CEN/TS 15531)
    /// • GTFS
    /// • VDV Standard (VDV 452, 455, 462, ...)
    ///     • IFOPT
    /// • ETSI / ISO Model (DENM, CAM, SPAT/MAP, IVI, …)
    /// • tpegML Model (TPEG2-TEC, TPEG2-PKI, ...)
    /// • DINO
    /// • INSPIRE data specification (according to Delegated Regulation (EC) No
    /// 1089/2010)
    /// • GML
    /// • other
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetPossibleAccMods()
    {
        return new[] { "Other", "GBFS", "GTFS", "DATEX II profile", "OpenAPI", "NeTEX", "MDS", "DATEX II Light", "VDV Standard", "http://publications.europa.eu/resource/authority/file-type/KML" };
    }

    public IEnumerable<string> GetPossibleAccInts()
    {
        return new[] { "OTS2", "http://publications.europa.eu/resource/authority/file-type/MSG_HTTP", "Other", "SOAP", "FTP" };
    }

    public IEnumerable<string> GetPossibleFrequencies()
    {
        return new[]
        {
            "Up to 1min", "On occurence", "Up to 5min", "Up to 10 min", "Up to 15min",
            "Up to 30 min", "Up to 1h", "Up to 2h", "Up to 3h", "Up to 12h", "Up to 24h",
            "Up to Weekly", "Up to Monthly", "Up to every 3month", "Up to every 6month", "Up to yearly",
            "Less frequent than yearly", "http://publications.europa.eu/resource/authority/frequency/DAILY",
            "http://publications.europa.eu/resource/authority/frequency/ANNUAL",
            "http://publications.europa.eu/resource/authority/frequency/QUARTERLY",
            "http://publications.europa.eu/resource/authority/frequency/MONTHLY",
            "http://publications.europa.eu/resource/authority/frequency/ANNUAL_2"
        };
    }

    public IEnumerable<string> GetPossibleAccCons()
    {
        return new[] { "Push", "Push periodic", "Pull", "Push on occurence" };
    }

    public IEnumerable<string> GetPossibleRegions()
    {
        return new[]
        {
            "http://data.europa.eu/nuts/code/BE3", "http://data.europa.eu/nuts/code/BE2",
            "http://data.europa.eu/nuts/code/BE1"
        };
    }
    
    private IEnumerable<Stakeholder>? _stakeholders;

    public async Task<IEnumerable<Stakeholder>> GetStakeholders()
    {
        if (_stakeholders != null) return _stakeholders;

        _stakeholders = await _stakeholderLoader.GetStakeholders(Path.Combine(_dataPath, "stakeholders", "2023"));
        
        return _stakeholders;
    }

    public async Task<IEnumerable<string>> GetTags()
    {
        var tagsTodayFile = "tags.json";
        var tagIds = await TryReadToday<Response<string[]>>(tagsTodayFile);
        if (tagIds == null)
        {
            tagIds = await _client.GetTagList();
            await WriteTodayAsync(tagsTodayFile, tagIds);
        }

        return tagIds.Result;
    }

    public async Task<IEnumerable<Organization>> GetOrganizations()
    {
        var organizationsTodayFile = "organizations.json";
        var organizationIds = await TryReadToday<Response<string[]>>(organizationsTodayFile);
        if (organizationIds == null)
        {
            organizationIds = await _client.GetOrganizationList();
            await WriteTodayAsync(organizationsTodayFile, organizationIds);
        }

        var organizations = new List<Organization>();
        foreach (var organizationId in organizationIds.Result)
        {
            var organizationTodayFile = Path.Combine("organizations", $"{organizationId}.json");
            var organization = await TryReadToday<Response<Organization>>(organizationTodayFile);
            if (organization == null)
            {
                organization = await _client.GetOrganization(organizationId);
                await WriteTodayAsync(organizationTodayFile, organization);
            }

            organizations.Add(organization.Result);
        }

        return organizations;
    }

    public async Task<IEnumerable<Package>> GetPackages()
    {
        var packagesTodayFile = "packages.json";
        var packageIds = await TryReadToday<Response<string[]>>(packagesTodayFile);
        if (packageIds == null)
        {
            packageIds = await _client.GetPackageList();
            await WriteTodayAsync(packagesTodayFile, packageIds);
        }

        var packages = new List<Package>();
        foreach (var packageId in packageIds.Result)
        {
            var packageTodayFile = Path.Combine("packages", $"{packageId}.json");
            var package = await TryReadToday<Response<Package>>(packageTodayFile);
            if (package == null)
            {
                package = await _client.GetPackage(packageId);
                await WriteTodayAsync(packageTodayFile, package);
            }

            packages.Add(package.Result);
        }

        return packages;
    }

    public async Task WriteResultAsync<T>(string file, IEnumerable<T> items)
    {
        var enumerable = items.ToList();
        var fileAtDataToday = Path.Combine(_todayPath, file);
        Excel.Write(fileAtDataToday, enumerable);
        var fileAtDataLatest = Path.Combine(_latestPath, file);
        Excel.Write(fileAtDataLatest, enumerable);
    }

    public async Task WriteDeclarationDocumentForOrganizationAsync(string file, Organization organization, Stream stream)
    {
        var organizationFolder = Path.Combine(_todayPath, "organizations");
        if (!Directory.Exists(organizationFolder)) Directory.CreateDirectory(organizationFolder);
        var declarations = Path.Combine(organizationFolder, "declarations");
        if (!Directory.Exists(declarations)) Directory.CreateDirectory(declarations);
        var documentFile = Path.Combine(declarations, $"{organization.Name}_{file}");
        await using (var outputStream = File.Open(documentFile, FileMode.Create))
        {
            await stream.CopyToAsync(outputStream);
        }

        organizationFolder = Path.Combine(_latestPath, "organizations");
        if (!Directory.Exists(organizationFolder)) Directory.CreateDirectory(organizationFolder);
        declarations = Path.Combine(organizationFolder, "declarations");
        if (!Directory.Exists(declarations)) Directory.CreateDirectory(declarations);
        documentFile = Path.Combine(declarations, $"{organization.Name}_{file}");
        stream.Seek(0, SeekOrigin.Begin);
        await using (var outputStream = File.Open(documentFile, FileMode.Create))
        {
            await stream.CopyToAsync(outputStream);
        }
    }

    private async Task<T?> TryReadToday<T>(string file)
    {
        var fileToday = Path.Combine(_todayPath, file);
        if (!File.Exists(fileToday)) return default;
        await using var stream = File.OpenRead(fileToday);
        return await JsonSerializer.DeserializeAsync<T>(stream);
    }

    private async Task WriteTodayAsync<T>(string file, T data)
    {
        var fileToday = Path.Combine(_todayPath, file);
        this.CreateDirectoryFor(fileToday);
        await using (var stream = File.Open(fileToday, FileMode.Create))
        {
            await JsonSerializer.SerializeAsync(stream, data);
        }
        var latestToday = Path.Combine(_latestPath, file);
        this.CreateDirectoryFor(latestToday);
        await using (var stream = File.Open(latestToday, FileMode.Create))
        {
            await JsonSerializer.SerializeAsync(stream, data);
        }
    }

    private void CreateDirectoryFor(string file)
    {
        var fileInfo = new FileInfo(file);
        switch (fileInfo.Directory)
        {
            case null:
                throw new Exception("Directory not found for file");
            case { Exists: true }:
                return;
            default:
                fileInfo.Directory.Create();
                break;
        }
    }
}