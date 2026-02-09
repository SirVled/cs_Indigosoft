using Indigosoft.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Storage
{
    public sealed record SourceStatus(
        ExchangeType Exchange,
        SourceState State,
        DateTime LastUpdateUtc
    );
}
