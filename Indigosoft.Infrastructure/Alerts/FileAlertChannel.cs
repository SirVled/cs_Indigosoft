using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Notifications
{
    public sealed class FileAlertChannel : IAlertChannel
    {
        private readonly string _path = "alerts.log";

        public Task SendAsync(Alert alert, CancellationToken ct)
        {
            return File.AppendAllTextAsync(
                _path,
                $"{alert.TimestampUtc:O} {alert.Message}{Environment.NewLine}",
                ct);
        }
    }
}
