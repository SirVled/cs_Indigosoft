
using cs_Indigosoft.HostedServices;
using Indigosoft.Application;
using Indigosoft.Application.Alerting;
using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Application.Services;
using Indigosoft.Infrastructure;
using Indigosoft.Infrastructure.Configuration;
using Indigosoft.Infrastructure.Sources;
using Indigosoft.Infrastructure.Sources.Binance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services
            .AddApplication(context.Configuration)
            .AddInfrastructure(context.Configuration);

        services.AddHostedService<SourcesHostedService>();
        services.AddHostedService<TickIngestionHostedService>();
        services.AddHostedService<SourceHealthHostedService>();

    })
    .Build();

await host.RunAsync();