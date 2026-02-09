using Indigosoft.Infrastructure.Configuration;
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
            services.AddSingleton<BinanceWsTickSource>();
            services.AddSingleton<BinanceRestTickSource>();
            services.AddSingleton<BybitWsTickSource>();
            services.AddSingleton<BybitRestTickSource>();

            return services;
        }
    }
}
