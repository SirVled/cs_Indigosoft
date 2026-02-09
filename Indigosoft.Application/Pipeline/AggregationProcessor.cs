using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Pipeline
{
    /// <summary>
    /// Передаёт тики в сервис агрегации.
    /// </summary>
    public sealed class AggregationProcessor : ITickProcessor
    {
        private readonly ITickAggregationService _aggregation;

        public AggregationProcessor(ITickAggregationService aggregation)
        {
            _aggregation = aggregation;
        }

        public ValueTask<Tick?> ProcessAsync(Tick tick, CancellationToken ct)
        {
            _aggregation.Process(tick);
            return ValueTask.FromResult<Tick?>(tick);
        }
    }
}
