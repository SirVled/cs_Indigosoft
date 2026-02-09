using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Application.Models;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Alerting
{
    public sealed class PriceAboveThresholdRule : IAlertRule
    {
        private readonly string _symbol;
        private readonly decimal _threshold;

        public PriceAboveThresholdRule(string symbol, decimal threshold)
        {
            _symbol = symbol;
            _threshold = threshold;
        }

        public Alert? Evaluate(Tick tick)
        {
            if (tick.Symbol == _symbol && tick.Price > _threshold)
            {
                return new Alert(
                    Code: "PRICE_HIGH",
                    Message: $"{tick.Symbol} price {tick.Price} > {_threshold}",
                    TimestampUtc: DateTime.UtcNow
                );
            }

            return null;
        }
    }
}
