using Indigosoft.Application.Interfaces.Metrics;
using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Indigosoft.Application.Pipeline
{
    public sealed class MetricsProcessor : ITickProcessor
    {
        private readonly IMetricsCollector _metrics;

        public MetricsProcessor(IMetricsCollector metrics)
        {
            _metrics = metrics;
        }

        public async ValueTask<Tick?> ProcessAsync(Tick tick, CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();

            _metrics.OnTickReceived(tick);

            sw.Stop();
            _metrics.OnTickProcessed(sw.Elapsed);

            return tick;
        }
    }
}
