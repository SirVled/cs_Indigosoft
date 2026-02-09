using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Infrastructure.Configuration
{
    public sealed class WebSocketSourceOptions
    {
        public string ApiKey { get; init; } = default;
        public string ApiSecret { get; init; } = default;
        public string Url { get; init; } = default!;
        public string[] Symbols { get; init; } = Array.Empty<string>();
        public int ReconnectDelaySeconds { get; init; } = 5;
    }
}
