using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NAP.AutoChecks;
using NAP.AutoChecks.API;
using NAP.AutoChecks.API.Stakeholders._2023;
using NAP.AutoChecks.API.Stakeholders._2025;
using NAP.AutoChecks.Evaluation0_DataValidation;
using NAP.AutoChecks.Evaluation1_1;
using NAP.AutoChecks.Evaluation1_2;
using NAP.AutoChecks.Evaluation1_2._2022;
using NAP.AutoChecks.Evaluation1_2._2023;
using NAP.AutoChecks.Evaluation2_1;
using NAP.AutoChecks.Evaluation2_2;
using NAP.AutoChecks.Evaluation2_2._2022;
using NAP.AutoChecks.Evaluation2_2._2023;
using NAP.AutoChecks.Queries;
using NAP.AutoChecks.Task1;
using NAP.AutoChecks.Task1.A;
using NAP.AutoChecks.Task1.B;
using NAP.AutoChecks.Task2;
using NAP.AutoChecks.Task3;
using NAP.AutoChecks.Task3._2025;
using Serilog;
using TransportDataBe.Client;

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
                var sampleDateString = hostingContext.Configuration.GetValue<string>("SamplingDate");
                if (!DateOnly.TryParse(sampleDateString, System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out var sampleDate))
                {
                    sampleDate = System.DateOnly.FromDateTime(DateTime.Today);
                }

                var apiKey = hostingContext.Configuration.GetValue<string>("ApiKey");
                services.AddTransportDataClient(s =>
                {
                    s.ApiKey = apiKey;
                });
                services.AddSingleton(new DataHandlerSettings()
                {
                    DataPath = dataPath,
                    SampleDay = sampleDate,
                });
                services.AddSingleton<DataHandler>();
                services.AddSingleton<StakeholderLoader2025>();
                
                services.AddSingleton<CheckStakeholderHasPackages>();
                services.AddSingleton<CheckStakeholdersRegistered>();
                services.AddSingleton<Task1>();
                
                services.AddSingleton<Task2>();

                services.AddSingleton<RandomSelection>();
                services.AddSingleton<Task3>();
            }).UseConsoleLifetime().Build();

        using var scope = host.Services.CreateScope();
        
        var task1 = scope.ServiceProvider.GetRequiredService<Task1>();
        await task1.Run();

        var task2 = scope.ServiceProvider.GetRequiredService<Task2>();
        await task2.Run();

        var task3 = scope.ServiceProvider.GetRequiredService<Task3>();
        await task3.Run();
        
        var allDeclarations = scope.ServiceProvider.GetRequiredService<StakeholdersAllDeclarations>();
        await allDeclarations.Get();

        var proxyAgreements = scope.ServiceProvider.GetRequiredService<OrganizationsGetProxyAgreements>();
        await proxyAgreements.Get();
    }
}