using Indigosoft.Application.Interfaces.Metrics;
using Indigosoft.Application.Models;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Indigosoft.Application.Services
{
    public sealed class MetricsCollector : IMetricsCollector
    {
        private long _totalTicks;
        private long _intervalTicks;

        private long _processingTicks;
        private long _processingCount;

        private long _lagTicks;
        private long _maxLagTicks;

        private readonly Stopwatch _interval = Stopwatch.StartNew();
        private readonly ConcurrentDictionary<string, long> _byExchange = new();

        public void OnTickReceived(Tick tick)
        {
            Interlocked.Increment(ref _totalTicks);
            Interlocked.Increment(ref _intervalTicks);

            _byExchange.AddOrUpdate(
                tick.Exchange.ToString(),
                1,
                (_, v) => v + 1);

            var lag = DateTime.UtcNow - tick.Timestamp;
            Interlocked.Add(ref _lagTicks, lag.Ticks);

            UpdateMaxLag(lag.Ticks);
        }

        public void OnTickProcessed(TimeSpan processingTime)
        {
            Interlocked.Add(ref _processingTicks, processingTime.Ticks);
            Interlocked.Increment(ref _processingCount);
        }

        public MetricsSnapshot SnapshotAndReset()
        {
            var elapsed = _interval.Elapsed.TotalSeconds;
            _interval.Restart();

            var intervalTicks = Interlocked.Exchange(ref _intervalTicks, 0);
            var processingTicks = Interlocked.Exchange(ref _processingTicks, 0);
            var processingCount = Interlocked.Exchange(ref _processingCount, 0);
            var lagTicks = Interlocked.Exchange(ref _lagTicks, 0);
            var maxLag = Interlocked.Exchange(ref _maxLagTicks, 0);

            return new MetricsSnapshot
            {
                TotalTicks = Interlocked.Read(ref _totalTicks),
                TicksPerSecond = elapsed > 0 ? intervalTicks / elapsed : 0,
                AvgProcessingTime = processingCount > 0
                    ? TimeSpan.FromTicks(processingTicks / processingCount)
                    : TimeSpan.Zero,
                AvgLag = intervalTicks > 0
                    ? TimeSpan.FromTicks(lagTicks / intervalTicks)
                    : TimeSpan.Zero,
                MaxLag = TimeSpan.FromTicks(maxLag),
                TicksByExchange = new Dictionary<string, long>(_byExchange)
            };
        }

        private void UpdateMaxLag(long lagTicks)
        {
            long current;
            do
            {
                current = _maxLagTicks;
                if (lagTicks <= current)
                    return;
            }
            while (Interlocked.CompareExchange(
                ref _maxLagTicks, lagTicks, current) != current);
        }
    }
}
