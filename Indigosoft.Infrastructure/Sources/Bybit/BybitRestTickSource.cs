using Indigosoft.Domain.Interfaces;
using Indigosoft.Domain.Models;
using Indigosoft.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Sources.Bybit
{
    internal class BybitRestTickSource : RestTickSourceBase
    {
        private IOptions<RestSourceOptions> _options;
        public BybitRestTickSource(IOptions<RestSourceOptions> options)
        {
            _options = options;
        }

        protected override Task<IEnumerable<Tick>> FetchTicksAsync(CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<Tick>>(Array.Empty<Tick>());
        }
    }
}
