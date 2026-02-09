using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Interfaces.Storage
{
    public interface ITickRepository
    {
        Task AddAsync(Tick tick, CancellationToken ct);
    }
}
