using Indigosoft.Domain.Interfaces;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Sources
{
    public abstract class RestTickSourceBase : ITickSource
    {

        protected RestTickSourceBase()
        {
        }

        public IAsyncEnumerable<Tick> StreamAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected abstract Task<IEnumerable<Tick>> FetchTicksAsync(CancellationToken ct);
    }
}
