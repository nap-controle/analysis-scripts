// See https://aka.ms/new-console-template for more information

using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NAP.AutoChecks;
using NAP.AutoChecks.API;
using NAP.AutoChecks.Checks;
using NAP.AutoChecks.Domain;
using NAP.AutoChecks.Evaluation1_1;
using NAP.AutoChecks.Evaluation1_2;
using NAP.AutoChecks.Evaluation1_2._2022;
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

                var dataPath = hostingContext.Configuration.GetValue<string>("DataPath");

                services.AddTransportDataClient();
                services.AddSingleton(new DataHandlerSettings()
                {
                    DataPath = dataPath
                });
                services.AddSingleton<DataHandler>();
                services.AddSingleton<OrganizationsNotInStakeholdersCheck>();
                services.AddSingleton<StakeholderHasPackagesCheck>();
                services.AddSingleton<RequiredFieldsFilledInCheck>();
                services.AddSingleton<StakeholderHasDeclarations>();
                services.AddSingleton<RandomizeDatasets>();
                services.AddSingleton<StakeholdersWithDeclarations>();
                services.AddSingleton<StakeholdersRegisteredCheck>();
                services.AddSingleton<RandomizeOrganizationsWithDeclarations>();

                services.AddSingleton(new StratifiedSamplingSetting());
                services.AddSingleton<StratifiedSampling>();
                services.AddSingleton<PreviouslySelectedDatasetLoader>();
                services.AddSingleton(new PreviouslySelectedDatasetsSettings()
                {
                    DataPath = dataPath
                });

                services.AddSingleton<AllStakeholders>();
                services.AddSingleton<StakeholdersAllDeclarations>();

                services.AddSingleton<Evaluation1_1>();
                services.AddSingleton<Evaluation1_2>();
            }).UseConsoleLifetime().Build();

        using var scope = host.Services.CreateScope();

        var evaluation1_1 = scope.ServiceProvider.GetRequiredService<Evaluation1_1>();
        await evaluation1_1.Run();
        
        var evaluation1_2 = scope.ServiceProvider.GetRequiredService<Evaluation1_2>();
        await evaluation1_2.Run();
    }
}