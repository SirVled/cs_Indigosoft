using Indigosoft.Domain.Entities;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Processing
{
    /// <summary>
    /// Выполняет агрегацию входящих тиков по временным интервалам (OHLCV).
    /// Хранит активные агрегаты в памяти и возвращает завершённые окна.
    /// </summary>
    public interface ITickAggregationService
    {
        void Process(Tick tick);

        /// <summary>
        /// Возвращает завершённые агрегаты и очищает их из памяти.
        /// </summary>
        IReadOnlyCollection<AggregatedCandle> Flush(DateTime utcNow);
    }
}
