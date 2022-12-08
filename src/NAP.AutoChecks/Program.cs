// See https://aka.ms/new-console-template for more information

using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NAP.AutoChecks;
using NAP.AutoChecks.API;
using NAP.AutoChecks.Checks;
using NAP.AutoChecks.Domain;
using NAP.AutoChecks.Queries;
using NAP.AutoChecks.Sampling;
using Serilog;
using Serilog.Formatting.Json;
using TransportDataBe.Client;
using TransportDataBe.Client.Models;

public static class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            args = new[] { "/Users/xivk/work/nap/analysis-scripts/data", "" };
        }
        var dataPath = args[0];
        
        var loggerFactory = new LoggerFactory();
        loggerFactory.AddSerilog();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostingContext, services) =>
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .CreateLogger();
                services.AddLogging(b =>
                {
                    b.ClearProviders();
                    b.AddSerilog();
                });

                services.AddTransportDataClient();
                services.AddSingleton(new DataHandlerSettings()
                {
                    DataPath = dataPath
                });
                services.AddSingleton<DataHandler>();
                services.AddSingleton<OrganizationsNotInStakeholders>();
                services.AddSingleton<StakeholderHasPackages>();
                services.AddSingleton<RequiredFieldsFilledIn>();
                services.AddSingleton<StakeholderHasDeclarations>();
                services.AddSingleton<RandomizeDatasets>();
                services.AddSingleton<StakeholdersWithDeclarations>();
                services.AddSingleton<StakeholdersWithoutOrganization>();
                services.AddSingleton<RandomizeOrganizationsWithDeclarations>();

                services.AddSingleton<StakeholdersAllDeclarations>();
            }).UseConsoleLifetime().Build();

        using var scope = host.Services.CreateScope();

        var download1 = scope.ServiceProvider.GetRequiredService<StakeholdersAllDeclarations>();
        await download1.Get();

        var check1 = scope.ServiceProvider.GetRequiredService<OrganizationsNotInStakeholders>();
        await check1.Check();

        var check2 = scope.ServiceProvider.GetRequiredService<StakeholderHasPackages>();
        await check2.Check();

        var check3 = scope.ServiceProvider.GetRequiredService<RequiredFieldsFilledIn>();
        await check3.Check();

        var check4 = scope.ServiceProvider.GetRequiredService<StakeholderHasDeclarations>();
        await check4.Check();

        var check5 = scope.ServiceProvider.GetRequiredService<StakeholdersWithoutOrganization>();
        await check5.Check();

        var sampling1 = scope.ServiceProvider.GetRequiredService<RandomizeDatasets>();
        await sampling1.Run();

        var sampling2 = scope.ServiceProvider.GetRequiredService<StakeholdersWithDeclarations>();
        await sampling2.Run();

        var sampling3 = scope.ServiceProvider.GetRequiredService<RandomizeOrganizationsWithDeclarations>();
        await sampling3.Run();
    }
}