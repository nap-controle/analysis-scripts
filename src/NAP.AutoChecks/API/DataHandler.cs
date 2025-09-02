using System.Collections.ObjectModel;
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
    private readonly string _sampleDayPath;
    private readonly string _latestPath;
    private readonly string _dataPath;
    private readonly ILogger<DataHandler> _logger;
    private readonly StakeholderLoader _stakeholderLoader;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {    
        PropertyNameCaseInsensitive = true
    };

    public DataHandler(Client client, DataHandlerSettings dataHandlerSettings, ILogger<DataHandler> logger, StakeholderLoader stakeholderLoader)
    {
        _client = client;
        _logger = logger;
        _stakeholderLoader = stakeholderLoader;

        _dataPath = dataHandlerSettings.DataPath ?? throw new Exception("Data path not set");
        _sampleDayPath = Path.Combine(dataHandlerSettings.DataPath,
            FormattableString.Invariant($"{dataHandlerSettings.SampleDay:yyyy-MM-dd}"));
        if (!Directory.Exists(_sampleDayPath)) Directory.CreateDirectory(_sampleDayPath);
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
    
    public static readonly HashSet<string> PossibleFormats = [
        "http://publications.europa.eu/resource/authority/file-type/XML",
        "http://publications.europa.eu/resource/authority/file-type/JSON",
        "http://publications.europa.eu/resource/authority/file-type/CSV",
        "http://publications.europa.eu/resource/authority/file-type/MSG_HTTP",
        "http://publications.europa.eu/resource/authority/file-type/PDF",
        "http://publications.europa.eu/resource/authority/file-type/XLS",
        "http://publications.europa.eu/resource/authority/file-type/XLSX",
        "http://publications.europa.eu/resource/authority/file-type/HTML",
        "http://publications.europa.eu/resource/authority/file-type/ZIP",
        "http://publications.europa.eu/resource/authority/file-type/WMS_SRVC",
        "http://publications.europa.eu/resource/authority/file-type/WFS_SRVC",
        "http://publications.europa.eu/resource/authority/file-type/GTFS",
        "http://publications.europa.eu/resource/authority/file-type/ATOM",
        "http://publications.europa.eu/resource/authority/file-type/GEOJSON",
        "http://publications.europa.eu/resource/authority/file-type/GEOTIFF",
        "http://publications.europa.eu/resource/authority/file-type/GML",
        "http://publications.europa.eu/resource/authority/file-type/GPKG",
        "http://publications.europa.eu/resource/authority/file-type/JSON_LD",
        "http://publications.europa.eu/resource/authority/file-type/PARQUET",
        "http://publications.europa.eu/resource/authority/file-type/REST",
        "http://publications.europa.eu/resource/authority/file-type/RSS"
    ];

    public static readonly HashSet<string> PossibleDataModels =
    [
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/datex-II",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/ocit-c",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/netex",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/siri",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/gtfs",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/gbfs",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/c-its",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/tpegml",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/dino",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/other",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/tn-its",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/gtfs-rt",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/gml",
        "https://w3id.org/mobilitydcat-ap/mobility-data-standard/inspire"
    ];


    public static readonly HashSet<string> PossibleApplicationLayerProtocols =
    [
        "https://w3id.org/mobilitydcat-ap/application-layer-protocol/soap",
        "https://w3id.org/mobilitydcat-ap/application-layer-protocol/ots2",
        "https://w3id.org/mobilitydcat-ap/application-layer-protocol/http-https",
        "https://w3id.org/mobilitydcat-ap/application-layer-protocol/ftp",
        "https://w3id.org/mobilitydcat-ap/application-layer-protocol/rss",
        "https://w3id.org/mobilitydcat-ap/application-layer-protocol/amqp",
        "https://w3id.org/mobilitydcat-ap/application-layer-protocol/mqtt",
        "https://w3id.org/mobilitydcat-ap/application-layer-protocol/grpc",
        "https://w3id.org/mobilitydcat-ap/application-layer-protocol/other",
        "https://w3id.org/mobilitydcat-ap/application-layer-protocol/ocit"
    ];

    public static readonly HashSet<string> PossibleAccessConditions =
    [
        "https://w3id.org/mobilitydcat-ap/conditions-for-access-and-usage/fee-required",
        "https://w3id.org/mobilitydcat-ap/conditions-for-access-and-usage/free-of-charge"
    ];

    public static readonly HashSet<string> PossibleUsageConditions =
    [  
        "https://w3id.org/mobilitydcat-ap/conditions-for-access-and-usage/contractual-arrangement",
        "https://w3id.org/mobilitydcat-ap/conditions-for-access-and-usage/licence-provided"
    ];
    
    public static readonly HashSet<string> PossibleLicenseTypes =
    [  
        "http://publications.europa.eu/resource/authority/licence/CC_BY_4_0",
        "http://publications.europa.eu/resource/authority/licence/CC_BYSA_4_0",
        "http://publications.europa.eu/resource/authority/licence/CC_BYNC_4_0",
        "http://publications.europa.eu/resource/authority/licence/CC0",
        "http://publications.europa.eu/resource/authority/licence/ODC_BY",
        "http://publications.europa.eu/resource/authority/licence/OGL_3_0",
        "http://publications.europa.eu/resource/authority/licence/CC_PDM_1_0",
        "http://publications.europa.eu/resource/authority/licence/ODC_PDDL",
        "http://publications.europa.eu/resource/authority/licence/GNU_FDL_1_3",
        "http://publications.europa.eu/resource/authority/licence/ODC_BL",
        "http://publications.europa.eu/resource/authority/licence/CC_BYNCND_4_0",
        "http://publications.europa.eu/resource/authority/licence/CC_BYNCSA_4_0",
        "http://publications.europa.eu/resource/authority/licence/CC_BYND_4_0",
        "Other"
    ];

    public IEnumerable<string> GetPossibleAccCons()
    {
        return new[] { "Push", "Push periodic", "Pull", "Push on occurence" };
    }

    public static IEnumerable<string> PossibleRegions =
    [
        "http://data.europa.eu/nuts/code/BE3", 
        "http://data.europa.eu/nuts/code/BE2",
        "http://data.europa.eu/nuts/code/BE1"
    ];

    public static readonly IReadOnlyDictionary<string, Dictionary<string, string>> PossibleFrequencies =
        new Dictionary<string, Dictionary<string, string>>
        {
            ["http://publications.europa.eu/resource/authority/frequency/IRREG"] = new Dictionary<string, string>
            {
                ["en"] = "On occurence", ["fr"] = "Dès que disponible", ["nl"] = "Zodra beschikbaar", ["de"] = "Sofort"
            },
            ["http://publications.europa.eu/resource/authority/frequency/1MIN"] = new Dictionary<string, string>
            {
                ["en"] = "Every minute", ["fr"] = "Toutes les minutes", ["nl"] = "Elke minuut", ["de"] = "Minütlich"
            },
            ["http://publications.europa.eu/resource/authority/frequency/5MIN"] = new Dictionary<string, string>
            {
                ["en"] = "Every five minutes", ["fr"] = "Toutes les 5 minutes", ["nl"] = "Om de vijf minuten",
                ["de"] = "Alle fünf Minuten"
            },
            ["http://publications.europa.eu/resource/authority/frequency/10MIN"] = new Dictionary<string, string>
            {
                ["en"] = "Every ten minutes", ["fr"] = "Toutes les 10 minutes", ["nl"] = "Om de tien minuten",
                ["de"] = "Alle zehn Minuten"
            },
            ["http://publications.europa.eu/resource/authority/frequency/15MIN"] = new Dictionary<string, string>
            {
                ["en"] = "Every fifteen minutes", ["fr"] = "Toutes les 15 minutes", ["nl"] = "Om de vijftien minuten",
                ["de"] = "Viertelstündlich"
            },
            ["http://publications.europa.eu/resource/authority/frequency/30MIN"] = new Dictionary<string, string>
            {
                ["en"] = "Every thirty minutes", ["fr"] = "Toutes les 30 minutes", ["nl"] = "Om de dertig minuten",
                ["de"] = "Halbstündlich"
            },
            ["http://publications.europa.eu/resource/authority/frequency/HOURLY"] = new Dictionary<string, string>
                { ["en"] = "Hourly", ["fr"] = "Toutes les heures", ["nl"] = "Om het uur", ["de"] = "Stündlich" },
            ["http://publications.europa.eu/resource/authority/frequency/BIHOURLY"] = new Dictionary<string, string>
            {
                ["en"] = "Bihourly", ["fr"] = "Toutes les deux heures", ["nl"] = "Om de twee uur",
                ["de"] = "Alle zwei Stunden"
            },
            ["http://publications.europa.eu/resource/authority/frequency/TRIHOURLY"] = new Dictionary<string, string>
            {
                ["en"] = "Trihourly", ["fr"] = "Toutes les trois heures", ["nl"] = "Om de drie uur",
                ["de"] = "Alle drei Stunden"
            },
            ["http://publications.europa.eu/resource/authority/frequency/12HRS"] = new Dictionary<string, string>
            {
                ["en"] = "Every twelve hours", ["fr"] = "Toutes les 12 heures", ["nl"] = "Om de twaalf uur",
                ["de"] = "Alle zwölf Stunden"
            },
            ["http://publications.europa.eu/resource/authority/frequency/DAILY"] = new Dictionary<string, string>
                { ["en"] = "Daily", ["fr"] = "Quotidien", ["nl"] = "Dagelijks", ["de"] = "Täglich" },
            ["http://publications.europa.eu/resource/authority/frequency/WEEKLY"] = new Dictionary<string, string>
                { ["en"] = "Weekly", ["fr"] = "Hebdomadaire", ["nl"] = "Wekelijks", ["de"] = "Wöchentlich" },
            ["http://publications.europa.eu/resource/authority/frequency/MONTHLY"] = new Dictionary<string, string>
                { ["en"] = "Monthly", ["fr"] = "Mensuel", ["nl"] = "Maandelijks", ["de"] = "Monatlich" },
            ["http://publications.europa.eu/resource/authority/frequency/QUARTERLY"] = new Dictionary<string, string>
            {
                ["en"] = "Quarterly", ["fr"] = "Trimestriel", ["nl"] = "Driemaandelijks", ["de"] = "Vierteljährlich"
            },
            ["http://publications.europa.eu/resource/authority/frequency/ANNUAL_2"] = new Dictionary<string, string>
                { ["en"] = "Semiannual", ["fr"] = "Semestriel", ["nl"] = "Halfjaarlijks", ["de"] = "Halbjährlich" },
            ["http://publications.europa.eu/resource/authority/frequency/ANNUAL"] = new Dictionary<string, string>
                { ["en"] = "Annual", ["fr"] = "Annuel", ["nl"] = "Jaarlijks", ["de"] = "Jährlich" },
            ["http://publications.europa.eu/resource/authority/frequency/IRREG"] = new Dictionary<string, string>
            {
                ["en"] = "Less frequent than yearly", ["fr"] = "Moins qu'une fois par an",
                ["nl"] = "Minder vaak dan één keer per jaar", ["de"] = "Weniger häufig als einmal pro Jahr"
            },
            ["http://publications.europa.eu/resource/authority/frequency/IRREG"] = new Dictionary<string, string>
                { ["en"] = "Irregular", ["fr"] = "Irrégulier", ["nl"] = "Onregelmatig", ["de"] = "Unregelmäßig" },
        };
    
    private IEnumerable<Stakeholder>? _stakeholders;

    public async Task<IEnumerable<Stakeholder>> GetStakeholders()
    {
        if (_stakeholders != null) return _stakeholders;

        _stakeholders = await _stakeholderLoader.GetStakeholders(Path.Combine(_dataPath, "stakeholders", "2024"));
        
        return _stakeholders;
    }

    public IReadOnlyDictionary<string, List<string>> GetMobilityThemes()
    {
        return new Dictionary<string, List<string>>
        {
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/air-and-space-travel"] = new List<string>(),
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/cycle-network-data"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/network-closures-diversions",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/network-detailed-attributes",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/network-geometry-and-lane-character"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/dynamic-traffic-signs-and-regulations"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/bridge-closures-and-access-conditions",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/direction-of-travel-on-reversible-lanes",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/dynamic-overtaking-bans-on-heavy-goods-vehicles",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/dynamic-speed-limits",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/lane-closures-and-access-conditions",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/other-access-restrictions-and-traffic-regulations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/other-temporary-traffic-management-measures-or-plans",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/road-closures-and-access-conditions",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/tunnel-closures-and-access-conditions"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/filling-and-charging-stations"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/availability-of-charging-points-for-electric-vehicles",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/availability-of-filling-stations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/location-and-conditions-of-charging-points",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/location-and-conditions-of-filling-stations"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/freight-and-logistics"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/availability-of-delivery-areas",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/freight-delivery-regulations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/location-of-delivery-areas"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/general-information-for-trip-planning"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/address-identifiers",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/parameters-needed-to-calculate-costs",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/parameters-needed-to-calculate-environmental-factors",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/points-of-interest",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/topographic-places"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/other"] = new List<string>(),
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/parking-service-and-rest-area-information"] =
                new List<string>
                {
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/bike-parking-locations",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/car-parking-availability",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/car-parking-locations-and-conditions",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/park-and-ride-stops",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/service-and-rest-area-availability",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/service-and-rest-area-locations-and-conditions",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/truck-parking-availability",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/truck-parking-locations-and-conditions"
                },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/pedestrian-network-data"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/pedestrian-accessibility-facilities",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/pedestrian-network-geometry"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/public-transport-non-scheduled-transport"] =
                new List<string>
                {
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/accesibility-information-for-vehicles",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/environmental-standards-for-vehicles",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/fares",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/locations-and-stations",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/provider-data",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/reservation-and-purchase-options",
                    "https://w3id.org/mobilitydcat-ap/mobility-theme/service-areas-and-service-times"
                },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/public-transport-scheduled-transport"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/basic-commercial-conditions",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/basic-common-standard-fares",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/common-fare-products",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/connection-links",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/disruptions-delays-cancellations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/environmental-standards-for-vehicles",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/hours-of-operation",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/network-topology-and-routes-lines",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/operational-calendar",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/passenger-classes",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/planned-interchanges-between-scheduled-services",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/purchase-information",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/real-time-estimated-departure-and-arrival-times",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/special-fare-products",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/stop-facilities-accessibility-and-paths-within-facility",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/stop-facilities-geometry-and-map-layout",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/stop-facilities-location-and-features",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/stop-facilities-status-of-features",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/timetables-static",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/transport-operators",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/vehicle-details"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/real-time-traffic-data"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/current-travel-times",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/expected-delays",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/location-and-length-of-queues",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/predicted-travel-times",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/speed",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/traffic-data-at-border-crossings-to-third-countries",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/traffic-volume",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/waiting-time-at-border-crossings-to-non-eu-member-states"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/road-events-and-conditions"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/accidents-and-incidents",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/poor-road-conditions",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/road-weather-conditions"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/road-work-information"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/long-term-road-works",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/short-term-road-works"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/sharing-and-hiring-services"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/bike-hiring-availability",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/bike-hiring-stations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/bike-sharing-availability",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/bike-sharing-locations-and-stations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/car-hiring-availability",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/car-hiring-stations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/car-sharing-availability",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/car-sharing-locations-and-stations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/environmental-standards-for-vehicles",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/e-scooter-sharing-availability",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/e-scooter-sharing-locations-and-stations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/payment-methods"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/static-road-network-data"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/geometry",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/gradients",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/junctions",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/number-of-lanes",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/road-classification",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/road-width"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/static-traffic-signs-and-regulations"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/bridge-access-conditions",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/other-static-traffic-signs",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/other-traffic-regulations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/permanent-access-restrictions",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/speed-limits",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/traffic-circulation-plans",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/tunnel-access-conditions"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/toll-information"] = new List<string>
            {
                "https://w3id.org/mobilitydcat-ap/mobility-theme/applicable-road-user-charges-and-payment-methods",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/identification-of-tolled-roads",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/location-of-tolling-stations",
                "https://w3id.org/mobilitydcat-ap/mobility-theme/payment-methods-for-tolls"
            },
            ["https://w3id.org/mobilitydcat-ap/mobility-theme/waterways-and-water-bodies"] = new List<string>(),
        };
    }

    public static readonly HashSet<string> PossibleFluentTags =
    [
        "https://w3id.org/mobilitydcat-ap/transport-mode/air",
        "https://w3id.org/mobilitydcat-ap/transport-mode/long-distance-rail",
        "https://w3id.org/mobilitydcat-ap/transport-mode/regional-and-local-rail",
        "https://w3id.org/mobilitydcat-ap/transport-mode/long-distance-coach",
        "https://w3id.org/mobilitydcat-ap/transport-mode/maritime",
        "https://w3id.org/mobilitydcat-ap/transport-mode/metro-subway-train",
        "https://w3id.org/mobilitydcat-ap/transport-mode/tram-light-rail",
        "https://w3id.org/mobilitydcat-ap/transport-mode/bus",
        "https://w3id.org/mobilitydcat-ap/transport-mode/shuttle-bus",
        "https://w3id.org/mobilitydcat-ap/transport-mode/shuttle-ferry",
        "https://w3id.org/mobilitydcat-ap/transport-mode/taxi",
        "https://w3id.org/mobilitydcat-ap/transport-mode/car-sharing",
        "https://w3id.org/mobilitydcat-ap/transport-mode/car-pooling",
        "https://w3id.org/mobilitydcat-ap/transport-mode/car-hire",
        "https://w3id.org/mobilitydcat-ap/transport-mode/bike-sharing",
        "https://w3id.org/mobilitydcat-ap/transport-mode/bike-hire",
        "https://w3id.org/mobilitydcat-ap/transport-mode/ride-pooling",
        "https://w3id.org/mobilitydcat-ap/transport-mode/e-scooter",
        "https://w3id.org/mobilitydcat-ap/transport-mode/car",
        "https://w3id.org/mobilitydcat-ap/transport-mode/truck",
        "https://w3id.org/mobilitydcat-ap/transport-mode/motorcycle",
        "https://w3id.org/mobilitydcat-ap/transport-mode/bicycle",
        "https://w3id.org/mobilitydcat-ap/transport-mode/pedestrian",
        "https://w3id.org/mobilitydcat-ap/transport-mode/other"
    ];

        public static readonly IReadOnlyDictionary<string, Dictionary<string, string>> PossibleCountries =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["http://publications.europa.eu/resource/authority/country/BEL"] = new Dictionary<string, string>
                    { ["en"] = "Belgium", ["fr"] = "Belgique", ["nl"] = "België", ["de"] = "Belgien" },
                ["http://publications.europa.eu/resource/authority/country/NLD"] = new Dictionary<string, string>
                    { ["en"] = "Netherlands", ["fr"] = "Pays-Bas", ["nl"] = "Nederland", ["de"] = "Niederlande" },
                ["http://publications.europa.eu/resource/authority/country/FRA"] = new Dictionary<string, string>
                    { ["en"] = "France", ["fr"] = "France", ["nl"] = "Frankrijk", ["de"] = "Frankreich" },
                ["http://publications.europa.eu/resource/authority/country/DEU"] = new Dictionary<string, string>
                    { ["en"] = "Germany", ["fr"] = "Allemagne", ["nl"] = "Duitsland", ["de"] = "Deutschland" },
                ["http://publications.europa.eu/resource/authority/country/LUX"] = new Dictionary<string, string>
                    { ["en"] = "Luxembourg", ["fr"] = "Luxembourg", ["nl"] = "Luxemburg", ["de"] = "Luxemburg" },
                ["http://publications.europa.eu/resource/authority/country/GBR"] = new Dictionary<string, string>
                {
                    ["en"] = "United Kingdom", ["fr"] = "Royaume-Uni", ["nl"] = "Verenigd Koninkrijk",
                    ["de"] = "Vereinigtes Königreich"
                },
                ["http://publications.europa.eu/resource/authority/country/BGR"] = new Dictionary<string, string>
                    { ["en"] = "Bulgaria", ["fr"] = "Bulgarie", ["nl"] = "Bulgarije", ["de"] = "Bulgarien" },
                ["http://publications.europa.eu/resource/authority/country/CZE"] = new Dictionary<string, string>
                    { ["en"] = "Czechia", ["fr"] = "Tchéquie", ["nl"] = "Tsjechië", ["de"] = "Tschechien" },
                ["http://publications.europa.eu/resource/authority/country/DNK"] = new Dictionary<string, string>
                    { ["en"] = "Denmark", ["fr"] = "Danemark", ["nl"] = "Denemarken", ["de"] = "Dänemark" },
                ["http://publications.europa.eu/resource/authority/country/EST"] = new Dictionary<string, string>
                    { ["en"] = "Estonia", ["fr"] = "Estonie", ["nl"] = "Estland", ["de"] = "Estland" },
                ["http://publications.europa.eu/resource/authority/country/IRL"] = new Dictionary<string, string>
                    { ["en"] = "Ireland", ["fr"] = "Irlande", ["nl"] = "Ierland", ["de"] = "Irland" },
                ["http://publications.europa.eu/resource/authority/country/GRC"] = new Dictionary<string, string>
                    { ["en"] = "Greece", ["fr"] = "Grèce", ["nl"] = "Griekenland", ["de"] = "Griechenland" },
                ["http://publications.europa.eu/resource/authority/country/ESP"] = new Dictionary<string, string>
                    { ["en"] = "Spain", ["fr"] = "Espagne", ["nl"] = "Spanje", ["de"] = "Spanien" },
                ["http://publications.europa.eu/resource/authority/country/HRV"] = new Dictionary<string, string>
                    { ["en"] = "Croatia", ["fr"] = "Croatie", ["nl"] = "Kroatië", ["de"] = "Kroatien" },
                ["http://publications.europa.eu/resource/authority/country/ITA"] = new Dictionary<string, string>
                    { ["en"] = "Italy", ["fr"] = "Italie", ["nl"] = "Italië", ["de"] = "Italien" },
                ["http://publications.europa.eu/resource/authority/country/CYP"] = new Dictionary<string, string>
                    { ["en"] = "Cyprus", ["fr"] = "Chypre", ["nl"] = "Cyprus", ["de"] = "Zypern" },
                ["http://publications.europa.eu/resource/authority/country/LVA"] = new Dictionary<string, string>
                    { ["en"] = "Latvia", ["fr"] = "Lettonie", ["nl"] = "Letland", ["de"] = "Lettland" },
                ["http://publications.europa.eu/resource/authority/country/LTU"] = new Dictionary<string, string>
                    { ["en"] = "Lithuania", ["fr"] = "Lituanie", ["nl"] = "Litouwen", ["de"] = "Litauen" },
                ["http://publications.europa.eu/resource/authority/country/HUN"] = new Dictionary<string, string>
                    { ["en"] = "Hungary", ["fr"] = "Hongrie", ["nl"] = "Hongarije", ["de"] = "Ungarn" },
                ["http://publications.europa.eu/resource/authority/country/MLT"] = new Dictionary<string, string>
                    { ["en"] = "Malta", ["fr"] = "Malte", ["nl"] = "Malta", ["de"] = "Malta" },
                ["http://publications.europa.eu/resource/authority/country/AUT"] = new Dictionary<string, string>
                    { ["en"] = "Austria", ["fr"] = "Autriche", ["nl"] = "Oostenrijk", ["de"] = "Österreich" },
                ["http://publications.europa.eu/resource/authority/country/POL"] = new Dictionary<string, string>
                    { ["en"] = "Poland", ["fr"] = "Pologne", ["nl"] = "Polen", ["de"] = "Polen" },
                ["http://publications.europa.eu/resource/authority/country/PRT"] = new Dictionary<string, string>
                    { ["en"] = "Portugal", ["fr"] = "Portugal", ["nl"] = "Portugal", ["de"] = "Portugal" },
                ["http://publications.europa.eu/resource/authority/country/ROU"] = new Dictionary<string, string>
                    { ["en"] = "Romania", ["fr"] = "Roumanie", ["nl"] = "Roemenië", ["de"] = "Rumänien" },
                ["http://publications.europa.eu/resource/authority/country/SVN"] = new Dictionary<string, string>
                    { ["en"] = "Slovenia", ["fr"] = "Slovénie", ["nl"] = "Slovenië", ["de"] = "Slowenien" },
                ["http://publications.europa.eu/resource/authority/country/SVK"] = new Dictionary<string, string>
                    { ["en"] = "Slovakia", ["fr"] = "Slovaquie", ["nl"] = "Slowakije", ["de"] = "Slowakei" },
                ["http://publications.europa.eu/resource/authority/country/FIN"] = new Dictionary<string, string>
                    { ["en"] = "Finland", ["fr"] = "Finlande", ["nl"] = "Finland", ["de"] = "Finnland" },
                ["http://publications.europa.eu/resource/authority/country/SWE"] = new Dictionary<string, string>
                    { ["en"] = "Sweden", ["fr"] = "Suède", ["nl"] = "Zweden", ["de"] = "Schweden" },
            };

    public static readonly IReadOnlyDictionary<string, Dictionary<string, string>> NUTS1_BE =
        new Dictionary<string, Dictionary<string, string>>
        {
            ["http://data.europa.eu/nuts/code/BE1"] = new Dictionary<string, string>
            {
                ["en"] = "RÉGION DE BRUXELLES-CAPITALE/BRUSSELS HOOFDSTEDELIJK GEWEST",
                ["fr"] = "RÉGION DE BRUXELLES-CAPITALE/BRUSSELS HOOFDSTEDELIJK GEWEST",
                ["nl"] = "RÉGION DE BRUXELLES-CAPITALE/BRUSSELS HOOFDSTEDELIJK GEWEST",
                ["de"] = "RÉGION DE BRUXELLES-CAPITALE/BRUSSELS HOOFDSTEDELIJK GEWEST"
            },
            ["http://data.europa.eu/nuts/code/BE2"] = new Dictionary<string, string>
            {
                ["en"] = "VLAAMS GEWEST", ["fr"] = "VLAAMS GEWEST", ["nl"] = "VLAAMS GEWEST", ["de"] = "VLAAMS GEWEST"
            },
            ["http://data.europa.eu/nuts/code/BE3"] = new Dictionary<string, string>
            {
                ["en"] = "RÉGION WALLONNE", ["fr"] = "RÉGION WALLONNE", ["nl"] = "RÉGION WALLONNE",
                ["de"] = "RÉGION WALLONNE"
            },
        };

    public async Task<IEnumerable<Organization>> GetOrganizations()
    {
        var organizationsTodayFile = "organizations.json";
        var organizationIdsJson = await TryReadToday(organizationsTodayFile);
        if (organizationIdsJson == null)
        {
            organizationIdsJson = await _client.GetOrganizationList();
            await WriteTodayAsync(organizationsTodayFile, organizationIdsJson);
        }
        var organizationIds = JsonSerializer.Deserialize<Response<string[]>>(organizationIdsJson, 
            _jsonSerializerOptions) ?? throw new Exception("Could not read json");

        var organizations = new List<Organization>();
        foreach (var organizationId in organizationIds.Result)
        {
            if (Constants.OrganizationsBlacklist.Contains(organizationId)) continue;
            
            var organizationTodayFile = Path.Combine("organizations", $"{organizationId}.json");
            var organizationJson = await TryReadToday(organizationTodayFile);
            if (organizationJson == null)
            {
                organizationJson = await _client.GetOrganization(organizationId);
                await WriteTodayAsync(organizationTodayFile, organizationJson);
            }
            var organization = JsonSerializer.Deserialize<Response<Organization>>(organizationJson, 
                _jsonSerializerOptions) ?? throw new Exception("Could not read json");

            organizations.Add(organization.Result);
        }

        return organizations;
    }

    public async Task<IEnumerable<Package>> GetPackages()
    {
        var packagesTodayFile = "packages.json";
        var packageIdsJson = await TryReadToday(packagesTodayFile);
        if (packageIdsJson == null)
        {
            packageIdsJson = await _client.GetPackageList();
            await WriteTodayAsync(packagesTodayFile, packageIdsJson);
        }
        var packageIds = JsonSerializer.Deserialize<Response<string[]>>(packageIdsJson, 
            _jsonSerializerOptions) ?? throw new Exception("Could not read json");

        var packages = new List<Package>();
        foreach (var packageId in packageIds.Result)
        {
            var packageTodayFile = Path.Combine("packages", $"{packageId}.json");
            var packageJson = await TryReadToday(packageTodayFile);
            if (packageJson == null)
            {
                packageJson = await _client.GetPackage(packageId);
                await WriteTodayAsync(packageTodayFile, packageJson);
            }
            var package = JsonSerializer.Deserialize<Response<Package>>(packageJson, 
                _jsonSerializerOptions) ?? throw new Exception("Could not read json");

            packages.Add(package.Result);
        }

        return packages;
    }

    public async Task WriteResultAsync<T>(string file, IEnumerable<T> items)
    {
        var enumerable = items.ToList();
        var fileAtDataToday = Path.Combine(_sampleDayPath, file);
        Excel.Write(fileAtDataToday, enumerable);
        var fileAtDataLatest = Path.Combine(_latestPath, file);
        Excel.Write(fileAtDataLatest, enumerable);
    }

    public async Task WriteDeclarationDocumentForOrganizationAsync(string file, Organization organization, Stream stream)
    {
        var organizationFolder = Path.Combine(_sampleDayPath, "organizations");
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

    public async Task WriteProxyAgreementForOrganizationAsync(string file, Organization organization, Stream stream)
    {
        var organizationFolder = Path.Combine(_sampleDayPath, "organizations");
        if (!Directory.Exists(organizationFolder)) Directory.CreateDirectory(organizationFolder);
        var proxy_agreemtns = Path.Combine(organizationFolder, "proxy_agreements");
        if (!Directory.Exists(proxy_agreemtns)) Directory.CreateDirectory(proxy_agreemtns);
        var documentFile = Path.Combine(proxy_agreemtns, $"{organization.Name}_{file}");
        await using (var outputStream = File.Open(documentFile, FileMode.Create))
        {
            await stream.CopyToAsync(outputStream);
        }

        organizationFolder = Path.Combine(_latestPath, "organizations");
        if (!Directory.Exists(organizationFolder)) Directory.CreateDirectory(organizationFolder);
        proxy_agreemtns = Path.Combine(organizationFolder, "proxy_agreements");
        if (!Directory.Exists(proxy_agreemtns)) Directory.CreateDirectory(proxy_agreemtns);
        documentFile = Path.Combine(proxy_agreemtns, $"{organization.Name}_{file}");
        stream.Seek(0, SeekOrigin.Begin);
        await using (var outputStream = File.Open(documentFile, FileMode.Create))
        {
            await stream.CopyToAsync(outputStream);
        }
    }

    private async Task<string?> TryReadToday(string file)
    {
        var fileToday = Path.Combine(_sampleDayPath, file);
        if (!File.Exists(fileToday)) return null;
        return await File.ReadAllTextAsync(fileToday);
    }

    private async Task WriteTodayAsync(string file, string data)
    {
        var fileToday = Path.Combine(_sampleDayPath, file);
        this.CreateDirectoryFor(fileToday);
        await File.WriteAllTextAsync(fileToday, data);
        
        var latestToday = Path.Combine(_latestPath, file);
        this.CreateDirectoryFor(latestToday);
        await File.WriteAllTextAsync(latestToday, data);
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