using System;
using System.Collections.Generic;
using System.Text;
using Indigosoft.Application.Interfaces.Alerting;

namespace Indigosoft.Infrastructure.Notifications
{
    public class EmailAlertService : IAlertService
    {
        public Task SendAsync(string message)
        {
            throw new NotImplementedException();
        }
    }
}
