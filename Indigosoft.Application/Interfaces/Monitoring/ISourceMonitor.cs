using Indigosoft.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Monitoring
{
    /// <summary>
    /// Отвечает за отслеживание состояния источников данных.
    /// </summary>
    public interface ISourceMonitor
    {
        void MarkTick(ExchangeType exchange, DateTime timestampUtc);
        SourceState GetState(ExchangeType exchange);
    }
}
