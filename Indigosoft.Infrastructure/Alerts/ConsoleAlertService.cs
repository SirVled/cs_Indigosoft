using Indigosoft.Application.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Notifications
{
    public class ConsoleAlertService : IAlertService
    {
        public Task SendAsync(string message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
