using Indigosoft.Application.Interfaces.Monitoring;
using Indigosoft.Domain.Enums;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace cs_Indigosoft.HostedServices
{
    public sealed class SourceHealthHostedService : BackgroundService
    {
        private readonly ISourceMonitor _monitor;

        public SourceHealthHostedService(ISourceMonitor monitor)
        {
            _monitor = monitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    foreach (ExchangeType exchange in Enum.GetValues(typeof(ExchangeType)))
                    {
                        var state = _monitor.GetState(exchange);
                        Console.WriteLine($"[{DateTime.UtcNow:O}] {exchange}: {state}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
