using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Domain.Interfaces
{
    public interface IAlertRule
    {
        bool IsTriggered(Tick tick);
    }
}
