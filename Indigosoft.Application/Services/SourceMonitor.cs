using Indigosoft.Application.Interfaces.Monitoring;
using Indigosoft.Domain.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Services
{
    public sealed class SourceMonitor : ISourceMonitor
    {
        private readonly ConcurrentDictionary<ExchangeType, DateTime> _lastTickUtc = new();

        private static readonly TimeSpan DegradedThreshold = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan OfflineThreshold = TimeSpan.FromSeconds(10);

        public void MarkTick(ExchangeType exchange, DateTime timestampUtc)
        {
            _lastTickUtc[exchange] = timestampUtc;
        }

        public SourceState GetState(ExchangeType exchange)
        {
            if (!_lastTickUtc.TryGetValue(exchange, out var last))
                return SourceState.Offline;

            var lag = DateTime.UtcNow - last;

            if (lag <= DegradedThreshold)
                return SourceState.Online;

            if (lag <= OfflineThreshold)
                return SourceState.Degraded;

            return SourceState.Offline;
        }
    }
}
