using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Notifications
{
    public sealed class ConsoleAlertChannel : IAlertChannel
    {
        public Task SendAsync(Alert alert, CancellationToken ct)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[ALERT] {alert.TimestampUtc:O} {alert.Message}");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}
