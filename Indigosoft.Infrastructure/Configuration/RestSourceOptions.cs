using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Configuration
{
    public sealed class RestSourceOptions
    {
        public string BaseUrl { get; init; } = default!;
        public int PollingIntervalSeconds { get; init; } = 1;
    }
}
