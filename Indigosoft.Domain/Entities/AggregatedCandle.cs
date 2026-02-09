using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Domain.Entities
{
    public sealed record AggregatedCandle(
         string Symbol,
         DateTime PeriodStart,
         TimeSpan Interval,
         decimal Open,
         decimal High,
         decimal Low,
         decimal Close,
         decimal Volume
     );
}
