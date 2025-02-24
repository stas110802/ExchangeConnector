using ExchangeConnector.ExchangeClients.Clients.Rest;
using ExchangeConnector.ExchangeClients.Types;

namespace ExchangeConnector.ExchangeClients.UnitTest;

public class BinanceRestClientTest
{
    private IRestExchangeConnector _client;
    private const int MaxCount = 20;
    private const string CurrencyPair = "BTCUSDT";
    
    [SetUp]
    public void StartUp()
    {
        _client = new BinanceRestClient(
            ExchangeApiOptionsFactory.CreateBinanceRestApiOptions());
    }

    [Test]
    public async Task TradesTest()
    {
        var trades = await _client.GetNewTradesAsync(CurrencyPair, MaxCount);
        if (trades.Count() > MaxCount ||
            !trades.Any(x => x.Price > 1))
            Assert.Fail();

        Assert.Pass();
    }
    
    [Test]
    public async Task CandlesTest()
    {
        var candles = await _client.GetCandleSeriesAsync(CurrencyPair, CandleType.ThirtyMin, count: MaxCount);
        if (candles.Count() > MaxCount ||
            !candles.Any(x => x.TotalVolume > 1))
            Assert.Fail();
        
        Assert.Pass();
    }
    
    [Test]
    public async Task TickerTest()
    {
        var ticker = await _client.GetTickerAsync(CurrencyPair);
        if (ticker.LastPrice == 0 || ticker.Volume == 0)
            Assert.Fail();
        
        Assert.Pass();
    }
    
    [Test]
    public async Task SpreadTest()
    {
        var spread = await _client.CalculateMonthSpreadAsync(CurrencyPair);
        if (spread == 0)
            Assert.Fail();
        
        Assert.Pass();
    }
}