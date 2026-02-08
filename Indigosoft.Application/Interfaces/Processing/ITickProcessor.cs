using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Processing
{
    public interface ITickProcessor
    {
        ValueTask<Tick?> ProcessAsync(Tick tick, CancellationToken ct);
    }
}
