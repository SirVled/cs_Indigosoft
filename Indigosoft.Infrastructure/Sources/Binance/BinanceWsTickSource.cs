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

namespace Indigosoft.Infrastructure.Sources.Binance
{
    public sealed class BinanceWsTickSource
    : WebSocketTickSourceBase, IStoppableSource
    {
        private readonly WebSocketSourceOptions _options;
        private ClientWebSocket? _socket;
        private readonly SemaphoreSlim _closeLock = new(1, 1);

        public BinanceWsTickSource(
            IOptionsMonitor<WebSocketSourceOptions> options)
        {
            _options = options.Get("Binance");
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
                    await _socket.ConnectAsync(
                        new Uri(_options.Url), ct);

                    await SubscribeAsync(ct);
                    await ReceiveLoopAsync(ct);
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

        private async Task SubscribeAsync(CancellationToken ct)
        {
            var streams = _options.Symbols
                .Select(s => $"{s.ToLowerInvariant()}@trade");

            var payload = JsonSerializer.Serialize(new
            {
                method = "SUBSCRIBE",
                @params = streams,
                id = 1
            });

            var bytes = Encoding.UTF8.GetBytes(payload);

            await _socket!.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: ct);
        }

        private async Task ReceiveLoopAsync(CancellationToken ct)
        {
            var buffer = new byte[16_384];

            while (!ct.IsCancellationRequested &&
                   _socket!.State == WebSocketState.Open)
            {
                var result = await _socket.ReceiveAsync(buffer, ct);

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

                if (!root.TryGetProperty("data", out var data))
                    return;

                var symbol = data.GetProperty("s").GetString()!;
                var price = decimal.Parse(data.GetProperty("p").GetString()!, CultureInfo.InvariantCulture);
                var volume = decimal.Parse(data.GetProperty("q").GetString()!, CultureInfo.InvariantCulture);
                var ts = DateTimeOffset
                    .FromUnixTimeMilliseconds(
                        data.GetProperty("T").GetInt64())
                    .UtcDateTime;

                Publish(new Tick(
                    symbol,
                    price,
                    volume,
                    ts,
                    ExchangeType.Binance
                ));
            }
            catch (Exception ex)
            {
                // некорректный JSON / не тик — игнорируем
                Console.WriteLine(ex.Message);
            }
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