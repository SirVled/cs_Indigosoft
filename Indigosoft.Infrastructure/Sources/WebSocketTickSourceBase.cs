using Indigosoft.Domain.Interfaces;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Indigosoft.Infrastructure.Sources
{
    public abstract class WebSocketTickSourceBase : ITickSource
    {
        private readonly Channel<Tick> _channel =
            Channel.CreateUnbounded<Tick>();

        public virtual async IAsyncEnumerable<Tick> StreamAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation]
        CancellationToken ct)
        {
            await foreach (var tick in _channel.Reader.ReadAllAsync(ct))
                yield return tick;
        }

        protected void Publish(Tick tick)
        {
            _channel.Writer.TryWrite(tick);
        }

        protected void Complete(Exception? ex = null)
        {
            _channel.Writer.Complete(ex);
        }
    }
}
