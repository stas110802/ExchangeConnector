using System.Globalization;
using System.Text;
using ExchangeConnector.ExchangeClients.Endpoints;
using ExchangeConnector.ExchangeClients.Models;
using ExchangeConnector.ExchangeClients.Options;
using ExchangeConnector.ExchangeClients.RestApi;
using ExchangeConnector.ExchangeClients.Types;
using ExchangeConnector.ExchangeClients.Utilities;
using Newtonsoft.Json.Linq;

namespace ExchangeConnector.ExchangeClients.Clients.Rest;

public sealed class BinanceRestClient : IRestExchangeConnector
{
    private readonly BaseRestApi<BinanceRequest> _restApi;

    public BinanceRestClient(ExchangeApiOptions options)
    {
        _restApi = new BaseRestApi<BinanceRequest>(options);
    }

    public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
    {
        if (maxCount is < 1 or > 1000)
            throw new ArgumentOutOfRangeException(nameof(maxCount), maxCount, "Max count must be between 1 and 1000");

        var query = $"?symbol={pair}&limit={maxCount}";
        var response = (await _restApi
                .CreateRequest(MethodType.Get, BinanceEndpoint.Trades, query)
                .ExecuteAsync())
            .FromJson<JToken>();

        var result = new List<Trade>();
        foreach (var trade in response)
        {
            var time = trade["time"]
                .GetDateTimeFromUTimeMilliseconds();

            var side = SideType.Buy;
            if (trade["isBuyerMaker"].ToObject<bool>() == false)
                side = SideType.Sell;

            result.Add(new Trade
            {
                Price = trade["price"].ToObject<decimal>(),
                Amount = trade["qty"].ToObject<decimal>(),
                Pair = pair,
                Side = side,
                Time = time,
                Id = trade["id"].ToString()
            });
        }

        return result;
    }

    public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, CandleType type,
        DateTimeOffset? startTime = null, DateTimeOffset? endTime = null, long? count = 0)
    {
        var queryBuilder = new StringBuilder();
        queryBuilder.Append($"?symbol={pair}&interval={type.Value}");

        if (startTime != null && endTime != null)
            queryBuilder.Append($"&startTime={startTime}&endTime={endTime}");

        if (count is > 0 and < 1000)
            queryBuilder.Append($"&limit={count}");

        var query = queryBuilder.ToString();
        var response = (await _restApi
                .CreateRequest(MethodType.Get, BinanceEndpoint.Candles, query)
                .ExecuteAsync())
            .FromJson<JToken>();

        var result = new List<Candle>();
        foreach (var item in response)
        {
            var time = item[0].GetDateTimeFromUTimeMilliseconds();
            result.Add(new Candle
            {
                OpenPrice = item[1].ToObject<decimal>(),
                HighPrice = item[2].ToObject<decimal>(),
                LowPrice = item[3].ToObject<decimal>(),
                ClosePrice = item[4].ToObject<decimal>(),
                TotalVolume = item[5].ToObject<decimal>(),
                OpenTime = time,
                Pair = pair
            });
        }

        return result;
    }

    public async Task<Ticker> GetTickerAsync(string pair)
    {
        try
        {
            var query = $"?symbol={pair}&type=MINI";
            var response = (await _restApi
                    .CreateRequest(MethodType.Get, BinanceEndpoint.Ticker, query)
                    .ExecuteAsync())
                .FromJson<JToken>();
            return new Ticker
            {
                Symbol = pair,
                LastPrice = response["lastPrice"].ToObject<decimal>(),
                HighPrice = response["highPrice"].ToObject<decimal>(),
                LowPrice = response["lowPrice"].ToObject<decimal>(),
                Volume = response["volume"].ToObject<decimal>()
            };
        }
        catch (Exception)
        {
            return new Ticker();
        }
    }

    public async Task<decimal> CalculateMonthSpreadAsync(string pair)
    {
        var currentPrice = (await GetTickerAsync(pair)).LastPrice;
        var futurePrice = await GetFuturePriceBySomeAlgoAsync(pair);

        return futurePrice - currentPrice;
    }

    private async Task<decimal> GetFuturePriceBySomeAlgoAsync(string symbol)
    {
        var historicalPrices = await GetSomePricesAsync(symbol);
        var averagePrice = historicalPrices.Average();

        return averagePrice;
    }
    
    private async Task<List<decimal>> GetSomePricesAsync(string symbol)
    {
        var prices = new List<decimal>();

        var query = $"?symbol={symbol}&interval=1d&limit=300&startTime={GetStartTimeForDaysAgo(30)}";
        var response = await _restApi
            .CreateRequest(MethodType.Get, BinanceEndpoint.Candles, query)
            .ExecuteAsync();

        var json = response.FromJson<JArray>();
        if (json.Count <= 0) return prices;

        prices.AddRange(json.Select(item =>
                decimal.Parse(item[4].ToString(), CultureInfo.InvariantCulture)));

        return prices;
    }

    private long GetStartTimeForDaysAgo(int daysAgo)
    {
        var date = DateTime.UtcNow.AddDays(-daysAgo).Date;

        return new DateTimeOffset(date).ToUnixTimeMilliseconds();
    }
}