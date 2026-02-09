using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Services
{
    public sealed class AlertService : IAlertService
    {
        private readonly IEnumerable<IAlertChannel> _channels;

        public AlertService(IEnumerable<IAlertChannel> channels)
        {
            _channels = channels;
        }

        public async Task HandleAsync(Alert alert, CancellationToken ct)
        {
            foreach (var channel in _channels)
            {
                await channel.SendAsync(alert, ct);
            }
        }
    }
}
