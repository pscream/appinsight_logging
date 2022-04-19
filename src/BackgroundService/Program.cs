using System;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Kubernetes.Debugging;

using BackgroundService.Services;

namespace BackgroundService
{
    class Program
    {

        private static async Task Main(string[] args)
        {

            var channel = new InMemoryChannel();

            var builder = Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.AddCommandLine(args);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    //logging.AddApplicationInsights("d7bc97a8-dda4-4aea-b010-f43fd1710167");
                })
                .ConfigureServices((hostContext, services) =>
                {

                    services.AddHostedService<LoggingService>();

                    // Uncomment the next 2 lines if you need to figure out why Kubernetes logging doesn't add custom dimensions with the 'Kubernetes.' prefixes
                    //var observer = new ApplicationInsightsKubernetesDiagnosticObserver(DiagnosticLogLevel.Trace);
                    //ApplicationInsightsKubernetesDiagnosticSource.Instance.Observable.SubscribeWithAdapter(observer);

                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.AddApplicationInsightsKubernetesEnricher();
                    services.Configure<TelemetryConfiguration>(
                    (config) =>
                    {
                        config.TelemetryChannel = channel;
                    }
);

                }).UseConsoleLifetime();

            IHost host = builder.Build();

            try
            {
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                var logger = host?.Services.GetRequiredService<ILogger<Program>>();
                logger?.LogCritical(ex.ToString());
                throw;
            }
            finally
            {
                channel.Flush();
            }

        }

    }

}
