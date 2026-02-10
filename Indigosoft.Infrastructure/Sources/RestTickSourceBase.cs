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

        public async IAsyncEnumerable<Tick> StreamAsync(
           [System.Runtime.CompilerServices.EnumeratorCancellation]
            CancellationToken cancellationToken)
        {
            yield break;
        }

        protected abstract Task<IEnumerable<Tick>> FetchTicksAsync(CancellationToken ct);
    }
}
