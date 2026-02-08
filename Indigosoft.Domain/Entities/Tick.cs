using Indigosoft.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Domain.Models
{
    public sealed record Tick(
        string Symbol,
        decimal Price,
        decimal Volume,
        DateTime Timestamp,
        ExchangeType Exchange
    );
}
