using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Configuration
{
    public sealed class RestSourceOptions
    {
        public string Url { get; init; } = default!;
        public string[] Symbols { get; init; } = Array.Empty<string>();
        public int PollingIntervalSeconds { get; init; } = 1;
    }
}
