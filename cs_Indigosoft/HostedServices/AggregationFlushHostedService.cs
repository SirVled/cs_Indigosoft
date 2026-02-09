using Indigosoft.Application.Interfaces.Processing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace cs_Indigosoft.HostedServices
{
    public sealed class AggregationFlushHostedService : BackgroundService
    {
        private readonly ITickAggregationService _aggregation;

        public AggregationFlushHostedService(
            ITickAggregationService aggregation)
        {
            _aggregation = aggregation;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var candles = _aggregation.Flush(DateTime.UtcNow);

                foreach (var candle in candles)
                {
                    // TODO: сохранить в БД
                    Console.WriteLine(
                        $"[CANDLE] {candle.Symbol} {candle.Interval} " +
                        $"O:{candle.Open} H:{candle.High} L:{candle.Low} C:{candle.Close}");
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
