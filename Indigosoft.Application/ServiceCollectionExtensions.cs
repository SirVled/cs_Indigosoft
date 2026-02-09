using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Application.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //Синглтоны - потому что консольное приложение
            services.AddSingleton<ITickProcessor, DeduplicationProcessor>();
            services.AddSingleton<ITickPipeline, TickPipeline>();

            return services;
        }
    }
}
