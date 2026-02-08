using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Processing
{
    public interface ITickPipeline
    {
        ValueTask ProcessAsync(Tick tick, CancellationToken ct);
    }
}
