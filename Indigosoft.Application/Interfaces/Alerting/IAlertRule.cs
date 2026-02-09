using Indigosoft.Application.Models;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Alerting
{
    /// <summary>
    /// Правило генерации алертов на основе входящих тиков.
    /// </summary>
    public interface IAlertRule
    {
        Alert? Evaluate(Tick tick);
    }
}
