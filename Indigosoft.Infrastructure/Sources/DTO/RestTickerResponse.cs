using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Sources.DTO
{
    public sealed class ResTickerResponse
    {
        public RestTickerResult? Result { get; set; }
    }

    public sealed class RestTickerResult
    {
        public List<RestTicker>? List { get; set; }
    }

    public sealed class RestTicker
    {
        public string Symbol { get; set; } = default!;
        public string LastPrice { get; set; } = default!;
        public string Volume24h { get; set; } = default!;
    }
}
