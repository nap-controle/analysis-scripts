namespace TransportDataBe.Client;

public class ClientSettings
{
    internal const string HttpClientName = "TransportData";

    public string Api { get; set; } = "https://transportdata.be/api/3/";

    public string ApiKey { get; set; }

    public string Website { get; set; } = "https://transportdata.be/";
}