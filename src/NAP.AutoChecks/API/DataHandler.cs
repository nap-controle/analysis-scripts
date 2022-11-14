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
    // • OCIT-C
    // • DATEX II Light
    // • NeTEx (CEN/TS 16614)
    // • SIRI (CEN/TS 15531)
    // • GTFS
    // • VDV Standard (VDV 452, 455, 462, ...)
    //     • IFOPT
    // • ETSI / ISO Model (DENM, CAM, SPAT/MAP, IVI, …)
    // • tpegML Model (TPEG2-TEC, TPEG2-PKI, ...)
    // • DINO
    // • INSPIRE data specification (according to Delegated Regulation (EC) No
    // 1089/2010)
    // • GML
    // • other

    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetPossibleAccMods()
    {
        return new[] { "Other", "GBFS", "GTFS", "DATEX II profile", "OpenAPI", "NeTEX", "MDS", "DATEX II Light", "VDV Standard" };
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

    public async Task<IEnumerable<Stakeholder>> GetStakeholders()
    {
        await using var stream =
            File.OpenRead(Path.Combine(_dataPath, "stakeholders", "organisations.csv"));
        var stakeholders = await Stakeholder.LoadFromCsv(stream);
        
        var stakeholdersMmtis = await Csv.ReadAsync<Stakeholder_MMTIS>(
            Path.Combine(_dataPath, "stakeholders", "organisations_MMTIS.csv"));
        foreach (var stakeholderMmtis in stakeholdersMmtis)
        {
            var stakeholder = stakeholders.FirstOrDefault(x => x.Id == stakeholderMmtis.Id);
            if (stakeholder == null) continue;

            stakeholder.IsMMTIS = stakeholderMmtis.IsMMTIS == "Yes";
        }
        
        var rttis = await Csv.ReadAsync<Stakeholder_RTTI>(
            Path.Combine(_dataPath, "stakeholders", "organisations_RTTI.csv"));
        foreach (var rtti in rttis)
        {
            var stakeholder = stakeholders.FirstOrDefault(x => x.Id == rtti.Id);
            if (stakeholder == null) continue;

            stakeholder.IsRTTI = rtti.IsRTTI == "Yes";
        }
        
        var srtis = await Csv.ReadAsync<Stakeholder_SRTI>(
            Path.Combine(_dataPath, "stakeholders", "organisations_SRTI.csv"));
        foreach (var srti in srtis)
        {
            var stakeholder = stakeholders.FirstOrDefault(x => x.Id == srti.Id);
            if (stakeholder == null) continue;

            stakeholder.IsSRTI = srti.IsSRTI == "Yes";
        }
        
        var sstps = await Csv.ReadAsync<Stakeholder_SSTP>(
            Path.Combine(_dataPath, "stakeholders", "organisations_SSTP.csv"));
        foreach (var sstp in sstps)
        {
            var stakeholder = stakeholders.FirstOrDefault(x => x.Id == sstp.Id);
            if (stakeholder == null) continue;

            stakeholder.IsSSTP = sstp.IsSSTP == "Yes";
        }

        return stakeholders;
    }

    public async Task<IEnumerable<string>> GetTags()
    {
        var tagsTodayFile = Path.Combine(_todayPath, "tags.json");
        var tagIds = await TryRead<Response<string[]>>(tagsTodayFile);
        if (tagIds == null)
        {
            tagIds = await _client.GetTagList();
            await Write(tagsTodayFile, tagIds);
        }

        return tagIds.Result;
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