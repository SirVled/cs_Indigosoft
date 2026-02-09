using Indigosoft.Domain.Enums;
using Indigosoft.Domain.Interfaces;
using Indigosoft.Domain.Models;
using Indigosoft.Infrastructure.Configuration;
using Indigosoft.Infrastructure.Sources.DTO;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;

namespace Indigosoft.Infrastructure.Sources.Bybit
{
    public sealed class BybitRestTickSource : ITickSource
    {
        private readonly HttpClient _http;
        private readonly RestSourceOptions _options;

        public BybitRestTickSource(
            HttpClient http,
            IOptionsMonitor<RestSourceOptions> options)
        {
            _http = http;
            _options = options.Get("Bybit");
        }

        public async IAsyncEnumerable<Tick> StreamAsync(
            [EnumeratorCancellation] CancellationToken ct)
        {
            // локальное состояние, чтобы не слать дубликаты
            var lastPrices = new Dictionary<string, decimal>();

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.PollingIntervalSeconds));

            while (await timer.WaitForNextTickAsync(ct))
            {
                foreach (var symbol in _options.Symbols)
                {
                    Tick? tick = null;

                    try
                    {
                        tick = await FetchTickAsync(symbol, ct);
                    }
                    catch (OperationCanceledException)
                    {
                        yield break;
                    }
                    catch
                    {
                        continue;
                    }

                    if (tick == null)
                        continue;

                    // простой dedup по цене
                    if (lastPrices.TryGetValue(symbol, out var last) &&
                        last == tick.Price)
                        continue;

                    lastPrices[symbol] = tick.Price;
                    yield return tick;
                }
            }
        }

        private async Task<Tick?> FetchTickAsync(string symbol, CancellationToken ct)
        {
            var url =
                $"{_options.Url}&symbol={symbol}";

            var response =
                await _http.GetFromJsonAsync<ResTickerResponse>(url, ct);

            var ticker = response?.Result?.List?.FirstOrDefault();
            if (ticker == null)
                return null;

            return new Tick(
                Symbol: symbol,
                Price: decimal.Parse(ticker.LastPrice),
                Volume: decimal.Parse(ticker.Volume24h),
                Timestamp: DateTime.UtcNow,
                Exchange: ExchangeType.Bybit
            );
        }
    }
}
