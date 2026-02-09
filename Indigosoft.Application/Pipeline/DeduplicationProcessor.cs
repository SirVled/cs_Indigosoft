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
        private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(10);

        public ValueTask<Tick?> ProcessAsync(Tick tick, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var key = $"{tick.Exchange}:{tick.Symbol}:{tick.Timestamp:O}";
            switch (tick.Exchange)
            {
                case Domain.Enums.ExchangeType.Binance:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case Domain.Enums.ExchangeType.Bybit:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case Domain.Enums.ExchangeType.Okx:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
            }
            
            Console.WriteLine($"{_seen.Count} {key}");
            Console.ResetColor();
            if (!_seen.TryAdd(key, now))
                return ValueTask.FromResult<Tick?>(null);

            Cleanup(now);
            return ValueTask.FromResult<Tick?>(tick);
        }

        private void Cleanup(DateTime now)
        {
            foreach (var pair in _seen)
            {
                if (now - pair.Value > Ttl)
                {
                    _seen.TryRemove(pair.Key, out _);
                }
            }
        }
    }
}
