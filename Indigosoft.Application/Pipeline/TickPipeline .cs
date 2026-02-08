using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigosoft.Application.Pipeline
{
    public sealed class TickPipeline : ITickPipeline
    {
        private readonly IReadOnlyList<ITickProcessor> _processors;

        public TickPipeline(IEnumerable<ITickProcessor> processors)
        {
            _processors = processors.ToList();
        }

        public async ValueTask ProcessAsync(Tick tick, CancellationToken ct)
        {
            Tick? current = tick;

            foreach (var processor in _processors)
            {
                if (current is null)
                    return;

                current = await processor.ProcessAsync(current, ct);
            }
        }
    }
}
