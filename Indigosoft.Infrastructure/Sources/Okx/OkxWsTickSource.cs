using Indigosoft.Application.Interfaces;
using Indigosoft.Domain.Enums;
using Indigosoft.Domain.Interfaces;
using Indigosoft.Domain.Models;
using Indigosoft.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace Indigosoft.Infrastructure.Sources.Okx
{
    public sealed class OkxWsTickSource :
    ITickSource,
    IStoppableSource
    {
        private readonly WebSocketSourceOptions _options;
        private readonly Channel<Tick> _channel =
            Channel.CreateUnbounded<Tick>();

        private ClientWebSocket? _socket;
        private readonly SemaphoreSlim _closeLock = new(1, 1);

        public OkxWsTickSource(
            IOptionsMonitor<WebSocketSourceOptions> options)
        {
            _options = options.Get("Okx");
        }

        public async IAsyncEnumerable<Tick> StreamAsync(
            [EnumeratorCancellation] CancellationToken ct)
        {
            _ = Task.Run(() => ConnectLoopAsync(ct), ct);

            await foreach (var tick in _channel.Reader.ReadAllAsync())
                yield return tick;
        }

        private async Task ConnectLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var socket = new ClientWebSocket();
                    _socket = socket;

                    await socket.ConnectAsync(
                        new Uri(_options.Url), ct);

                    await SubscribeAsync(socket, ct);
                    await ReceiveLoopAsync(socket, ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    await Task.Delay(_options.ReconnectDelaySeconds, ct);
                }
            }

            _channel.Writer.TryComplete();
        }

        private async Task SubscribeAsync(
            ClientWebSocket socket,
            CancellationToken ct)
        {
            var args = _options.Symbols.Select(s => new
            {
                channel = "tickers",
                instId = s
            });

            var payload = JsonSerializer.Serialize(new
            {
                op = "subscribe",
                args
            });

            var bytes = Encoding.UTF8.GetBytes(payload);
            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                ct);
        }

        private async Task ReceiveLoopAsync(
            ClientWebSocket socket,
            CancellationToken ct)
        {
            var buffer = new byte[16_384];

            try
            {
                while (!ct.IsCancellationRequested &&
                       socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer, ct);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ParseMessage(buffer.AsSpan(0, result.Count));
                }
            }
            catch (OperationCanceledException)
            {            
            }
            catch (WebSocketException)
            {
                // reconnect
            }
        }

        private void ParseMessage(ReadOnlySpan<byte> json)
        {
            try
            {
                using var doc = JsonDocument.Parse(new ReadOnlyMemory<byte>(json.ToArray()));
                var root = doc.RootElement;

                if (!root.TryGetProperty("arg", out var arg))
                    return;

                if (arg.GetProperty("channel").GetString() != "tickers")
                    return;

                var data = root.GetProperty("data")[0];

                var symbol = data.GetProperty("instId").GetString()!;
                var price = decimal.Parse(data.GetProperty("last").GetString()!, CultureInfo.InvariantCulture);
                var volume = decimal.Parse(data.GetProperty("vol24h").GetString()!, CultureInfo.InvariantCulture);

                var tick = new Tick(
                    Symbol: symbol,
                    Price: price,
                    Volume: volume,
                    Timestamp: DateTime.UtcNow,
                    Exchange: ExchangeType.Okx
                );

                _channel.Writer.TryWrite(tick);
            }
            catch { }
        }

        public async Task StopAsync()
        {
            await _closeLock.WaitAsync();
            try
            {
                if (_socket == null)
                    return;

                if (_socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
                {
                    try
                    {
                        await _socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Shutdown",
                            CancellationToken.None);
                    }
                    catch (WebSocketException)
                    {
                        // сокет уже мёртв
                    }
                }

                _socket.Dispose();
                _socket = null;
            }
            finally
            {
                _closeLock.Release();
            }
        }
    }

}
