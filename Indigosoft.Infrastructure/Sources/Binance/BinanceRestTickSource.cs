using Indigosoft.Domain.Interfaces;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Sources.Binance
{
    public class BinanceRestTickSource : RestTickSourceBase
    {
        public BinanceRestTickSource()
        {
        }

        protected override Task<IEnumerable<Tick>> FetchTicksAsync(CancellationToken ct)
        {
            //Заглушка
            //см. BybitRestTickSource
            return Task.FromResult<IEnumerable<Tick>>(Array.Empty<Tick>());
        }
    }
}
