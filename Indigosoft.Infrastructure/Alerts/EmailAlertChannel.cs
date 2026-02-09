using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Notifications
{
    public sealed class EmailAlertChannel : IAlertChannel
    {
        public Task SendAsync(Alert alert, CancellationToken ct)
        {
            // TODO: SMTP
            return Task.CompletedTask;
        }
    }
}
