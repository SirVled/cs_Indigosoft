using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Domain.Entities;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Services
{
    public sealed class TickAggregationService : ITickAggregationService
    {
        private static readonly TimeSpan[] Intervals =
        {
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromHours(1)
    };

        private readonly ConcurrentDictionary<string, CandleBuilder> _active = new();

        public void Process(Tick tick)
        {
            foreach (var interval in Intervals)
            {
                var periodStart = Align(tick.Timestamp, interval);
                var key = $"{tick.Symbol}:{interval}:{periodStart:O}";

                var builder = _active.GetOrAdd(
                    key,
                    _ => new CandleBuilder(tick.Symbol, periodStart, interval, tick.Exchange)
                );

                builder.Add(tick);
            }
        }

        public IReadOnlyCollection<AggregatedCandle> Flush(DateTime utcNow)
        {
            var completed = new List<AggregatedCandle>();

            foreach (var pair in _active)
            {
                var builder = pair.Value;
                if (utcNow >= builder.PeriodStart + builder.Interval)
                {
                    if (_active.TryRemove(pair.Key, out var finished))
                    {
                        completed.Add(finished.Build());
                    }
                }
            }

            return completed;
        }

        private static DateTime Align(DateTime timestamp, TimeSpan interval)
        {
            var ticks = timestamp.Ticks / interval.Ticks * interval.Ticks;
            return new DateTime(ticks, DateTimeKind.Utc);
        }
    }
}
