using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Models
{
    public sealed record Alert(
        string Code,
        string Message,
        DateTime TimestampUtc
    );
}
