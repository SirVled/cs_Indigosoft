using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Configuration
{
    public sealed class PriceAboveAlertOptions
    {
        public string Symbol { get; init; } = default!;
        public decimal Threshold { get; init; }
    }
}
