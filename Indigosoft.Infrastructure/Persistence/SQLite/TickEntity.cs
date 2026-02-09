using Indigosoft.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Persistence.SQLite
{
    public sealed class TickEntity
    {
        public long Id { get; set; }

        public string Symbol { get; set; } = default!;
        public decimal Price { get; set; }
        public decimal Volume { get; set; }

        public DateTime TimestampUtc { get; set; }
        public ExchangeType Exchange { get; set; }
    }
}
