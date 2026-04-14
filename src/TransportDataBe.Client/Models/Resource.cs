namespace TransportDataBe.Client.Models;

public class Resource
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Format { get; set; }

    public string? Acc_Mod { get; set; }
    public string? Acc_Int { get; set; }

    public string? Acc_Con { get; set; }

    public string? conditions_access { get; set; }

    public string? conditions_usage { get; set; }

    public string? license_type { get; set; }

    public TranslatedText? license_text_translated { get; set; }

    public string? Url { get; set; }
}