using Indigosoft.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Storage
{
    public interface ICandleRepository
    {
        Task AddBatchAsync(
            IReadOnlyCollection<AggregatedCandle> candles,
            CancellationToken ct);
    }
}
