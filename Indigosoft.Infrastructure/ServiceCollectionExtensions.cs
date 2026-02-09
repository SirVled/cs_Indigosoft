using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Domain.Interfaces;
using Indigosoft.Infrastructure.Configuration;
using Indigosoft.Infrastructure.Notifications;
using Indigosoft.Infrastructure.Sources.Binance;
using Indigosoft.Infrastructure.Sources.Bybit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Indigosoft.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // configs
            #region WebSocket
            services.AddOptions<WebSocketSourceOptions>("Binance")
                .Bind(configuration.GetSection("Sources:Binance:WebSocket"));

            services.AddOptions<WebSocketSourceOptions>("Bybit")
                .Bind(configuration.GetSection("Sources:Bybit:WebSocket"));
            #endregion

            #region Rest
            services.AddOptions<RestSourceOptions>("Binance")
                .Bind(configuration.GetSection("Sources:Binance:Rest"));

            services.AddOptions<RestSourceOptions>("Bybit")
                .Bind(configuration.GetSection("Sources:Bybit:Rest"));
            #endregion

            //Синглтоны - потому что консольное приложение
            // sources
            services.AddSingleton<ITickSource, BinanceWsTickSource>();
            services.AddSingleton<ITickSource, BinanceRestTickSource>();

            services.AddSingleton<ITickSource, BybitWsTickSource>();
            services.AddSingleton<ITickSource, BybitRestTickSource>();

            services.AddSingleton<IAlertChannel, ConsoleAlertChannel>();
            services.AddSingleton<IAlertChannel, FileAlertChannel>();
            services.AddSingleton<IAlertChannel, EmailAlertChannel>();

            return services;
        }
    }
}
