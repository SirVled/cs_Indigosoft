using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Application.Interfaces.Storage;
using Indigosoft.Domain.Interfaces;
using Indigosoft.Infrastructure.Configuration;
using Indigosoft.Infrastructure.Notifications;
using Indigosoft.Infrastructure.Persistence.SQLite;
using Indigosoft.Infrastructure.Persistence.SQLite.Repository;
using Indigosoft.Infrastructure.Sources.Binance;
using Indigosoft.Infrastructure.Sources.Bybit;
using Indigosoft.Infrastructure.Sources.Okx;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


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

            // sources
            services.AddSingleton<ITickSource, BinanceWsTickSource>();
           // services.AddSingleton<ITickSource, BinanceRestTickSource>();

            services.AddSingleton<ITickSource, BybitWsTickSource>();
            services.Configure<RestSourceOptions>(
            "Bybit",
            configuration.GetSection("Sources:Bybit:Rest"));

            services.AddHttpClient<BybitRestTickSource>(client =>
            {
                client.BaseAddress = new Uri("https://api.bybit.com");
                client.Timeout = TimeSpan.FromSeconds(5);
            });

            services.AddSingleton<ITickSource, BybitRestTickSource>();

            services.Configure<WebSocketSourceOptions>(
            "Okx",
            configuration.GetSection("Sources:Okx:WebSocket"));

            services.AddSingleton<ITickSource, OkxWsTickSource>();

            #region Alerts
            services.AddSingleton<IAlertChannel, ConsoleAlertChannel>();
            services.AddSingleton<IAlertChannel, FileAlertChannel>();
            services.AddSingleton<IAlertChannel, EmailAlertChannel>();
            #endregion

            services.AddDbContext<MarketDataDbContext>(o =>
            {
                var dbPath = Path.Combine(
                    AppContext.BaseDirectory,
                    "marketdata.db");

                o.UseSqlite($"Data Source={dbPath}");
            });

            services.AddScoped<ITickRepository, TickRepository>();
            services.AddScoped<ICandleRepository, CandleRepository>();

            return services;
        }
    }
}
