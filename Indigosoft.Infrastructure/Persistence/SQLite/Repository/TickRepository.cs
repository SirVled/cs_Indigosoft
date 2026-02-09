using Indigosoft.Application.Interfaces.Storage;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Persistence.SQLite.Repository
{
    public sealed class TickRepository : ITickRepository
    {
        private readonly MarketDataDbContext _db;

        public TickRepository(MarketDataDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Tick tick, CancellationToken ct)
        {
            _db.Ticks.Add(new TickEntity
            {
                Symbol = tick.Symbol,
                Price = tick.Price,
                Volume = tick.Volume,
                TimestampUtc = tick.Timestamp,
                Exchange = tick.Exchange
            });

            await _db.SaveChangesAsync(ct);
        }
    }
}
