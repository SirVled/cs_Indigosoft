using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Domain.Interfaces
{
    public interface ITickSource
    {
        IAsyncEnumerable<Tick> StreamAsync(CancellationToken cancellationToken);
    }
}
