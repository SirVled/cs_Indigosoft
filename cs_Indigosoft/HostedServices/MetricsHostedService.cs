using Indigosoft.Application.Interfaces.Metrics;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace cs_Indigosoft.HostedServices
{
    public sealed class MetricsHostedService : BackgroundService
    {
        private readonly IMetricsCollector _metrics;

        public MetricsHostedService(IMetricsCollector metrics)
        {
            _metrics = metrics;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var snapshot = _metrics.SnapshotAndReset();

                Console.WriteLine(
                    $"[METRICS] TPS={snapshot.TicksPerSecond:F1} " +
                    $"AvgLag={snapshot.AvgLag.TotalMilliseconds:F0}ms " +
                    $"MaxLag={snapshot.MaxLag.TotalMilliseconds:F0}ms " +
                    $"AvgProc={snapshot.AvgProcessingTime.TotalMicroseconds:F1}μs");

                foreach (var kv in snapshot.TicksByExchange)
                {
                    Console.WriteLine($"  {kv.Key}: {kv.Value}");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
