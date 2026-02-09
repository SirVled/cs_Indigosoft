using Indigosoft.Application.Interfaces.Monitoring;
using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Pipeline
{
    public sealed class MonitoringProcessor : ITickProcessor
    {
        private readonly ISourceMonitor _monitor;

        public MonitoringProcessor(ISourceMonitor monitor)
        {
            _monitor = monitor;
        }

        public ValueTask<Tick?> ProcessAsync(Tick tick, CancellationToken ct)
        {
            _monitor.MarkTick(tick.Exchange, tick.Timestamp);
            return ValueTask.FromResult<Tick?>(tick);
        }
    }
}
