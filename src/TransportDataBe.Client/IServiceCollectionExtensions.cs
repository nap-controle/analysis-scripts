using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace TransportDataBe.Client;

/// <summary>
/// Extensions methods for easier DI.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds the transport data client.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">A configure callback method.</param>
    public static void AddTransportDataClient(this IServiceCollection services,
        Action<ClientSettings>? configure = null)
    {
        var settings = new ClientSettings();
        configure?.Invoke(settings);

        services.AddHttpClient(ClientSettings.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.All,
            });

        services.AddSingleton(settings);
        services.AddSingleton<Client>();
    }
}