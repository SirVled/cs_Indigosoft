using Indigosoft.Application.Interfaces.Storage;
using Indigosoft.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace cs_Indigosoft.HostedServices
{
    public sealed class TickPersistenceHostedService : BackgroundService
    {
        private readonly TickIngestionService _ingestion;
        private readonly IServiceScopeFactory _scopeFactory;

        public TickPersistenceHostedService(
            TickIngestionService ingestion,
            IServiceScopeFactory scopeFactory)
        {
            _ingestion = ingestion;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var tick in _ingestion.Reader.ReadAllAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ITickRepository>();

                await repo.AddAsync(tick, stoppingToken);
            }
        }
    }
}
