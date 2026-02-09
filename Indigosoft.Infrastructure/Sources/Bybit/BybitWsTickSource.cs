using Indigosoft.Application.Interfaces;
using Indigosoft.Domain.Enums;
using Indigosoft.Domain.Interfaces;
using Indigosoft.Domain.Models;
using Indigosoft.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Indigosoft.Infrastructure.Sources.Bybit
{
    public class BybitWsTickSource : WebSocketTickSourceBase, IStoppableSource
    {
        private readonly WebSocketSourceOptions _options;
        private ClientWebSocket? _socket;       
        private readonly SemaphoreSlim _closeLock = new(1, 1); // Нужен чтобы не закрыть socket дважды (race-condition)
        public BybitWsTickSource(IOptionsMonitor<WebSocketSourceOptions> options)
        {
            _options = options.Get("Bybit");
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
                            "Application shutdown",
                            CancellationToken.None);
                    }
                    catch (WebSocketException)
                    {
                        // сокет уже мёртв — игнорируем
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
        public override async IAsyncEnumerable<Tick> StreamAsync(
            [EnumeratorCancellation] CancellationToken ct)
        {
            _ = Task.Run(() => ConnectAsync(ct), ct);

            await foreach (var tick in base.StreamAsync(ct))
                yield return tick;
        }

        public async Task ConnectAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    _socket = new ClientWebSocket();
                    await _socket.ConnectAsync(new Uri(_options.Url), ct);

                    await SubscribeAsync(_socket, ct);
                    await ReceiveLoopAsync(_socket, ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(_options.ReconnectDelaySeconds), ct);
                }
            }

            Complete();
        }


        private async Task SubscribeAsync(ClientWebSocket socket, CancellationToken ct)
        {
            var args = _options.Symbols
                .Select(s => $"tickers.{s.ToUpperInvariant()}");

            var payload = JsonSerializer.Serialize(new
            {
                op = "subscribe",
                args
            });

            var bytes = Encoding.UTF8.GetBytes(payload);
            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: ct);
        }

        private async Task ReceiveLoopAsync(ClientWebSocket socket, CancellationToken ct)
        {
            var buffer = new byte[16_384];

            while (!ct.IsCancellationRequested &&
                   socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, ct);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                TryHandleMessage(json);
            }
        }

        private void TryHandleMessage(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("topic", out var topic) ||
                    !topic.GetString()!.StartsWith("tickers."))
                    return;

                var data = root.GetProperty("data");

                var symbol = data.GetProperty("symbol").GetString()!;
                var price = decimal.Parse(data.GetProperty("lastPrice").GetString()!, CultureInfo.InvariantCulture);
                var volume = decimal.Parse(data.GetProperty("volume24h").GetString()!, CultureInfo.InvariantCulture);
                var ts = DateTimeOffset
                    .FromUnixTimeMilliseconds(
                        root.GetProperty("ts").GetInt64())
                    .UtcDateTime;

                Publish(new Tick(
                    symbol,
                    price,
                    volume,
                    ts,
                    ExchangeType.Bybit
                ));
            }
            catch(Exception ex)
            {
                // некорректный JSON / не тик — игнорируем
                Console.WriteLine(ex.Message);
            }
        }
    }
}
