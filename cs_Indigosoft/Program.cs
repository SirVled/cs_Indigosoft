using Indigosoft.Application.Providers;
using Indigosoft.Infrastructure.Provider.Rest;
using Indigosoft.Infrastructure.Provider.WS;

//var pipeline = new TickPipeline(processor);

var sources = new IMarketDataProvider[]
{
    new BinanceRestProvider(),
    new BybitRestProvider(),
    new BinanceSocketProvider(),
    new BybitSocketProvider()
};

Task.WhenAll(
  //  pipeline.StartAsync(ct),
    sources.Select(s => s.StartAsync(ct)),
 //   aggregationFlushService.StartAsync(ct)
);