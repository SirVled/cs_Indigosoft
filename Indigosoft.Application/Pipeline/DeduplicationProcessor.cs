using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Pipeline
{
    public sealed class DeduplicationProcessor : ITickProcessor
    {
        private readonly ConcurrentDictionary<string, DateTime> _seen = new();

        public ValueTask<Tick?> ProcessAsync(Tick tick, CancellationToken ct)
        {
            var key = $"{tick.Exchange}:{tick.Symbol}:{tick.Timestamp:O}";
            Console.WriteLine($"{_seen.Count} {tick.Price}");
            return _seen.TryAdd(key, tick.Timestamp)
                ? ValueTask.FromResult<Tick?>(tick)
                : ValueTask.FromResult<Tick?>(null);
        }
    }
}
