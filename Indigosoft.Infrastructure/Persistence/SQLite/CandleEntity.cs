using Indigosoft.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Persistence.SQLite
{
    public sealed class CandleEntity
    {
        public long Id { get; set; }

        public string Symbol { get; set; } = default!;
        public DateTime PeriodStartUtc { get; set; }
        public TimeSpan Interval { get; set; }

        public ExchangeType Exchange { get; set; }

        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
}
