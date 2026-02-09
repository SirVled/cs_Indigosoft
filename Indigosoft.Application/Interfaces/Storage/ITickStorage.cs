using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Storage
{
    public interface ITickStorage
    {
        ValueTask SaveAsync(Tick tick, CancellationToken ct);
    }
}
