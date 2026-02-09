using cs_Indigosoft.Extensions;
using Indigosoft.Application.Interfaces;
using Indigosoft.Application.Services;
using Indigosoft.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace cs_Indigosoft.HostedServices
{
    // <summary>
    /// Универсальный hosted-сервис, отвечающий за запуск всех источников
    /// рыночных данных и передачу тиков в ingestion pipeline.
    /// </summary>
    public sealed class MarketDataIngestionHostedService : BackgroundService
    {
        private readonly IEnumerable<ITickSource> _sources;
        private readonly TickIngestionService _ingestion;

        public MarketDataIngestionHostedService(
            IEnumerable<ITickSource> sources,
            TickIngestionService ingestion)
        {
            _sources = sources;
            _ingestion = ingestion;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var ingestionTask = _ingestion.RunAsync(stoppingToken);

            var sourceTasks = _sources.Select(source =>
                source.StartAsync(_ingestion.Writer, stoppingToken));

            await Task.WhenAll(sourceTasks);

            await ingestionTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var source in _sources)
            {
                if (source is IStoppableSource stoppable)
                {
                    await stoppable.StopAsync();
                }
            }

            _ingestion.Complete();

            await base.StopAsync(cancellationToken);
        }
    }
}
