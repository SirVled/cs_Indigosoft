
using Indigosoft.Application;
using Indigosoft.Application.Services;
using Indigosoft.Infrastructure;
using Indigosoft.Infrastructure.Configuration;
using Indigosoft.Infrastructure.Sources;
using Indigosoft.Infrastructure.Sources.Binance;
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
            .AddApplication()
            .AddInfrastructure(context.Configuration);  
    })
    .Build();

await host.RunAsync();