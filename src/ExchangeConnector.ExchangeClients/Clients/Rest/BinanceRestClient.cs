using System.Text;
using ExchangeConnector.ExchangeClients.Endpoints;
using ExchangeConnector.ExchangeClients.Interfaces;
using ExchangeConnector.ExchangeClients.Models;
using ExchangeConnector.ExchangeClients.Options;
using ExchangeConnector.ExchangeClients.RestApi;
using ExchangeConnector.ExchangeClients.Types;
using ExchangeConnector.ExchangeClients.Utilities;
using Newtonsoft.Json.Linq;
using static System.Decimal;

namespace ExchangeConnector.ExchangeClients.Clients.Rest;

public sealed class BinanceRestClient : IRestTestConnector
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
            if(trade["isBuyerMaker"].ToObject<bool>() == false)
                side = SideType.Sell;
            
            result.Add(new Trade
            {
                Pair = pair,
                Price = trade["price"].ToDecimal(),
                Amount = trade["qty"].ToDecimal(),
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
                OpenPrice = item[1].ToDecimal(),
                HighPrice = item[2].ToDecimal(),
                LowPrice = item[3].ToDecimal(),
                ClosePrice = item[4].ToDecimal(),
                TotalVolume = item[5].ToDecimal(),
                OpenTime = time,
                Pair = pair
            });
        }

        return result;
    }
}