using Indigosoft.Application.Interfaces.Processing;
using Indigosoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Indigosoft.Application.Services
{
    /// <summary>
    /// Принимает тики от нескольких источников и передаёт их
    /// в конвейер обработки через ограниченный асинхронный канал,
    /// обеспечивая контроль нагрузки и корректное завершение работы.
    /// </summary>
    public sealed class TickIngestionService
    {
        private readonly Channel<Tick> _channel;
        private readonly ITickPipeline _pipeline;

        public TickIngestionService(ITickPipeline pipeline, int capacity = 10_000)
        {
            _pipeline = pipeline;

            _channel = Channel.CreateBounded<Tick>(new BoundedChannelOptions(capacity)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            });
        }

        public ChannelWriter<Tick> Writer => _channel.Writer;

        public async Task RunAsync(CancellationToken ct)
        {
            await foreach (var tick in _channel.Reader.ReadAllAsync(ct))
            {
                await _pipeline.ProcessAsync(tick, ct);
            }
        }
    }
}
