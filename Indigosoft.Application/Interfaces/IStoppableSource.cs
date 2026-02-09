using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces
{
    public interface IStoppableSource
    {
        Task StopAsync();
    }
}
