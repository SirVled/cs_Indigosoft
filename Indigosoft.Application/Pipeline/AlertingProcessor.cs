using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Pipeline
{
    public sealed class AlertingProcessor : ITickProcessor
    {
        private readonly IEnumerable<IAlertRule> _rules;
        private readonly IAlertService _service;

        public AlertingProcessor(
            IEnumerable<IAlertRule> rules,
            IAlertService service)
        {
            _rules = rules;
            _service = service;
        }

        public async ValueTask<Tick?> ProcessAsync(Tick tick, CancellationToken ct)
        {
            foreach (var rule in _rules)
            {
                var alert = rule.Evaluate(tick);
                if (alert != null)
                    await _service.HandleAsync(alert, ct);
            }

            return tick;
        }
    }
}
