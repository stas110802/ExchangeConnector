using ExchangeConnector.ExchangeClients.Models;
using ExchangeConnector.ExchangeClients.Types;

namespace ExchangeConnector.ExchangeClients.Clients.Rest;

public interface IRestExchangeConnector
{
    Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount);
    Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, CandleType type, DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null, long? count = 0);
    Task<Ticker> GetTickerAsync(string pair);
    Task<decimal> CalculateMonthSpreadAsync(string pair);
}