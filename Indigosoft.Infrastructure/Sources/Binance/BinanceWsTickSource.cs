using Indigosoft.Domain.Interfaces;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Sources.Binance
{
    public class BinanceWsTickSource : WebSocketTickSourceBase
    {
        public Task ConnectAsync(CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}