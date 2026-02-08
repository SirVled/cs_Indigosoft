using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Alerting
{
    public interface IAlertService
    {
        public Task SendAsync(string message);
    }
}
