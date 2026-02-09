using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Models
{
    public sealed class MetricsSnapshot
    {
        public long TotalTicks { get; init; }

        public double TicksPerSecond { get; init; }

        public TimeSpan AvgProcessingTime { get; init; }

        public TimeSpan AvgLag { get; init; }
        public TimeSpan MaxLag { get; init; }

        public IReadOnlyDictionary<string, long> TicksByExchange { get; init; }
            = new Dictionary<string, long>();
    }
}
