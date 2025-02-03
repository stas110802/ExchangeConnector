using ExchangeConnector.ExchangeClients.Clients.Rest;
using ExchangeConnector.ExchangeClients.Interfaces;
using ExchangeConnector.ExchangeClients.Types;

namespace ExchangeConnector.ExchangeClients.UnitTest;

public class BinanceRestClientTest
{
    private IRestExchangeConnector _client;

    [SetUp]
    public void StartUp()
    {
        _client = new BinanceRestClient(
            ExchangeApiOptionsFactory.CreateBinanceRestApiOptions());
    }

    [Test]
    public async Task Test()
    {
        const int maxCount = 20;
        const string currencyPair = "BTCUSDT";
        var ticker = await _client.GetTickerAsync(currencyPair);
        if (ticker.LastPrice == 0 || ticker.Volume == 0)
            Assert.Fail();

        
        var trades = await _client.GetNewTradesAsync(currencyPair, maxCount);
        if (trades.Count() > maxCount ||
            !trades.Any(x => x.Price > 1))
            Assert.Fail();


        var candles = await _client.GetCandleSeriesAsync(currencyPair, CandleType.ThirtyMin, count: maxCount);
        if (candles.Count() > maxCount ||
            !candles.Any(x => x.TotalVolume > 1))
            Assert.Fail();

        Assert.Pass();
    }
}