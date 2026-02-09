using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Application.Interfaces.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace cs_Indigosoft.HostedServices
{
    /// <summary>
    /// Фоновый сервис, который периодически сбрасывает
    /// завершённые агрегированные свечи в базу данных.
    /// </summary>
    public sealed class AggregationFlushHostedService : BackgroundService
    {
        private readonly ITickAggregationService _aggregation;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AggregationFlushHostedService> _logger;

        public AggregationFlushHostedService(
            ITickAggregationService aggregation,
            IServiceScopeFactory scopeFactory,
            ILogger<AggregationFlushHostedService> logger)
        {
            _aggregation = aggregation;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await FlushAsync(stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                await FlushAsync(CancellationToken.None);
            }
        }

        private async Task FlushAsync(CancellationToken ct)
        {
            var candles = _aggregation.Flush(DateTime.UtcNow);
            if (candles.Count == 0)
                return;

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICandleRepository>();

            await repo.AddBatchAsync(candles, ct);

            _logger.LogInformation("Flushed {Count} candles", candles.Count);
        }
    }
}