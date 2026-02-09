using Indigosoft.Application.Interfaces.Alerting;
using Indigosoft.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Indigosoft.Infrastructure.Notifications
{
    public sealed class FileAlertChannel : IAlertChannel, IDisposable
    {
        private readonly Channel<string> _channel;
        private readonly Task _writerTask;
        private readonly StreamWriter _writer;

        public FileAlertChannel()
        {
            _channel = Channel.CreateUnbounded<string>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });

            _writer = new StreamWriter(
                new FileStream(
                    "alerts.log",
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.Read))
            {
                AutoFlush = true
            };

            _writerTask = Task.Run(WriterLoop);
        }

        public Task SendAsync(Alert alert, CancellationToken ct)
        {
            var line = $"{alert.TimestampUtc:O} {alert.Code} {alert.Message}";
            _channel.Writer.TryWrite(line);
            return Task.CompletedTask;
        }

        private async Task WriterLoop()
        {
            await foreach (var line in _channel.Reader.ReadAllAsync())
            {
                await _writer.WriteLineAsync(line);
            }
        }

        public void Dispose()
        {
            _channel.Writer.TryComplete();
            _writerTask.Wait();
            _writer.Dispose();
        }
    }
}
