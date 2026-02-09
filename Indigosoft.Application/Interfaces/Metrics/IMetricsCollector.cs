using Indigosoft.Application.Models;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Metrics
{
    public interface IMetricsCollector
    {
        void OnTickReceived(Tick tick);
        void OnTickProcessed(TimeSpan processingTime);

        MetricsSnapshot SnapshotAndReset();
    }
}
