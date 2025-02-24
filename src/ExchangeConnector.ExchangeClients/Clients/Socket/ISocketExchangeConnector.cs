using ExchangeConnector.ExchangeClients.Models;
using ExchangeConnector.ExchangeClients.Types;

namespace ExchangeConnector.ExchangeClients.Clients.Socket;

public interface ISocketExchangeConnector : IDisposable
{
    event Action<Trade> NewBuyTrade;
    event Action<Trade> NewSellTrade;
    void SubscribeTrades(string pair, int maxCount = 100);
    void UnsubscribeTrades(string pair);

    event Action<Candle> CandleSeriesProcessing;
    void SubscribeCandles(string pair, CandleType type, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0);
    void UnsubscribeCandles(string pair);
}