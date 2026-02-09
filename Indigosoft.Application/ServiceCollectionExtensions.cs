using Indigosoft.Application.Alerting;
using Indigosoft.Application.Configuration;
using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Application.Interfaces.Monitoring;
using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Application.Pipeline;
using Indigosoft.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //Синглтоны - потому что консольное приложение
            services.AddSingleton<ITickProcessor, DeduplicationProcessor>();
            services.AddSingleton<ITickPipeline, TickPipeline>();

            services.AddSingleton<ISourceMonitor, SourceMonitor>();
            services.AddSingleton<ITickProcessor, MonitoringProcessor>();

            services.AddSingleton<IAlertService, AlertService>();
            services.AddSingleton<ITickProcessor, AlertingProcessor>();

            services.AddSingleton<ITickAggregationService, TickAggregationService>();
            services.AddSingleton<ITickProcessor, AggregationProcessor>();

            #region Alerts
            var rules = configuration
                .GetSection("Alerts:PriceAbove")
                .Get<List<PriceAboveAlertOptions>>() ?? new();

            foreach (var rule in rules)
            {
                services.AddSingleton<IAlertRule>(
                    new PriceAboveThresholdRule(
                        rule.Symbol,
                        rule.Threshold
                    ));
            }
            #endregion

            services.AddSingleton<TickIngestionService>();          
            return services;
        }
    }
}
