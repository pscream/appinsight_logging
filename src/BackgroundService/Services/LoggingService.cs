using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;

namespace BackgroundService.Services
{

    internal class LoggingService : IHostedService
    {

        private readonly ILogger<LoggingService> _logger;
        private readonly TelemetryClient _tc;

        public LoggingService(ILogger<LoggingService> logger, TelemetryClient tc)
        {
            _logger = logger;
            _tc = tc;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting ...");

            await Task.Delay(TimeSpan.FromSeconds(3)); // We have to wait until Kubernetes properties are received

            _logger.LogError("This is an Error trial");

            _logger.LogCritical("This is an Critical trial");

            // NOTE. The next exception call is to check if it works. Please, comment if you don't need it anymore.
            try
            {
                throw new Exception("This is an Exception trial");
            }
            catch (Exception ex)
            {
                _tc.TrackException(ex);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping ...");

            return Task.CompletedTask;
        }

    }

}
