using Indigosoft.Domain.Interfaces;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace cs_Indigosoft.Extensions
{
    public static class TickSourceExtensions
    {
        public static async Task StartAsync(
            this ITickSource source,
            ChannelWriter<Tick> writer,
            CancellationToken ct)
        {
            await foreach (var tick in source.StreamAsync(ct))
            {
                await writer.WriteAsync(tick, ct);
            }
        }
    }
}
