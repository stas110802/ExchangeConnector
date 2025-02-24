using ExchangeConnector.ExchangeClients.Clients.Socket;
using ExchangeConnector.ExchangeClients.Models;
using ExchangeConnector.ExchangeClients.Types;

namespace ExchangeConnector.ExchangeClients.UnitTest;

public class BinanceWebSocketClientTest : IDisposable
{
    private ISocketExchangeConnector _client;
    private const string CurrencyPair = "ETHUSDT";

    [SetUp]
    public void StartUp()
    {
        _client = new BinanceSocketClient(
            ExchangeApiOptionsFactory.CreateBinanceWebSocketApiOptions());
    }

    [Test]
    public void TradesAndCandleTest()
    {
        var isSuccessful = true;
        _client.SubscribeTrades(CurrencyPair);
        _client.SubscribeCandles(CurrencyPair, CandleType.FifteenMin);

        _client.NewBuyTrade += trade => { TradeTest(trade, SideType.Buy); };
        _client.NewSellTrade += trade => { TradeTest(trade, SideType.Sell); };

        _client.CandleSeriesProcessing += candle =>
        {
            if(candle.OpenPrice <= 0)
                isSuccessful = false;
            var random = new Random().Next(0, 10);
            if (random == 5)
                _client.UnsubscribeCandles(CurrencyPair);
        };

        _client.UnsubscribeTrades(CurrencyPair);
        _client.UnsubscribeCandles(CurrencyPair);
        
        Assert.That(isSuccessful, Is.True);
        return;

        void TradeTest(Trade trade, SideType side)
        {
            if (trade.Side != side
                || trade.Price <= 0)
                isSuccessful = false;
            var random = new Random().Next(0, 10);
            if (random == 5)
                _client.UnsubscribeTrades(CurrencyPair);
            Thread.Sleep(500);
        }
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}