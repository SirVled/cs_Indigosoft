using Indigosoft.Application.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace cs_Indigosoft.HostedServices
{
    public sealed class TickIngestionHostedService : BackgroundService
    {
        private readonly TickIngestionService _ingestion;

        public TickIngestionHostedService(TickIngestionService ingestion)
        {
            _ingestion = ingestion;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("🚚 Tick ingestion consumer started");
            return _ingestion.RunAsync(stoppingToken);
        }
    }
}
