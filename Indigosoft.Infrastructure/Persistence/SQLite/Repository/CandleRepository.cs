using Indigosoft.Application.Interfaces.Storage;
using Indigosoft.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Persistence.SQLite.Repository
{
    public sealed class CandleRepository : ICandleRepository
    {
        private readonly MarketDataDbContext _db;

        public CandleRepository(MarketDataDbContext db)
        {
            _db = db;
        }

        public async Task AddBatchAsync(
            IReadOnlyCollection<AggregatedCandle> candles,
            CancellationToken ct)
        {
            foreach (var c in candles)
            {
                _db.Candles.Add(new CandleEntity
                {
                    Symbol = c.Symbol,
                    PeriodStartUtc = c.PeriodStart,
                    Interval = c.Interval,
                    Exchange = c.Exchange,
                    Open = c.Open,
                    High = c.High,
                    Low = c.Low,
                    Close = c.Close,
                    Volume = c.Volume
                });
            }

            await _db.SaveChangesAsync(ct);
        }
    }
}
