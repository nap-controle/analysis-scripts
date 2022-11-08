using System.Text.Json.Serialization;

namespace TransportDataBe.Client.Models;

public class Organization
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Name { get; set; }
}