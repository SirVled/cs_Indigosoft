using Indigosoft.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Alerting
{
    public interface IAlertChannel
    {
        Task SendAsync(Alert alert, CancellationToken ct);
    }
}
