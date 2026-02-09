using cs_Indigosoft.Extensions;
using Indigosoft.Application.Services;
using Indigosoft.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace cs_Indigosoft.HostedServices
{
    public sealed class SourcesHostedService : BackgroundService
    {
        private readonly IEnumerable<ITickSource> _sources;
        private readonly TickIngestionService _ingestion;

        public SourcesHostedService(
            IEnumerable<ITickSource> sources,
            TickIngestionService ingestion)
        {
            _sources = sources;
            _ingestion = ingestion;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = _sources
                .Select(source =>
                    source.StartAsync(_ingestion.Writer, stoppingToken));

            await Task.WhenAll(tasks);
        }
    }
}
