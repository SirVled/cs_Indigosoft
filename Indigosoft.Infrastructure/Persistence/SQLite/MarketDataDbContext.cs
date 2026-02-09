using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Persistence.SQLite
{
    public sealed class MarketDataDbContext : DbContext
    {
        public DbSet<TickEntity> Ticks => Set<TickEntity>();
        public DbSet<CandleEntity> Candles => Set<CandleEntity>();

        public MarketDataDbContext(DbContextOptions<MarketDataDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TickEntity>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => new { x.Exchange, x.Symbol, x.TimestampUtc });
            });

            modelBuilder.Entity<CandleEntity>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => new { x.Symbol, x.Interval, x.PeriodStartUtc });
            });
        }
    }
}
