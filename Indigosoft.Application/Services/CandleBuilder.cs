using Indigosoft.Domain.Entities;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Services
{
    internal sealed class CandleBuilder
    {
        public string Symbol { get; }
        public DateTime PeriodStart { get; }
        public TimeSpan Interval { get; }

        public decimal Open { get; private set; }
        public decimal High { get; private set; }
        public decimal Low { get; private set; }
        public decimal Close { get; private set; }
        public decimal Volume { get; private set; }

        private bool _initialized;

        public CandleBuilder(string symbol, DateTime periodStart, TimeSpan interval)
        {
            Symbol = symbol;
            PeriodStart = periodStart;
            Interval = interval;
        }

        public void Add(Tick tick)
        {
            if (!_initialized)
            {
                Open = High = Low = Close = tick.Price;
                _initialized = true;
            }
            else
            {
                High = Math.Max(High, tick.Price);
                Low = Math.Min(Low, tick.Price);
                Close = tick.Price;
            }

            Volume += tick.Volume;
        }

        public AggregatedCandle Build() =>
            new(Symbol, PeriodStart, Interval, Open, High, Low, Close, Volume);
    }
}
