using ExchangeConnector.ExchangeClients.Types;

namespace ExchangeConnector.ExchangeClients.Endpoints;

public class BinanceEndpoint : BaseType
{
    private BinanceEndpoint(string value) : base(value) { }

    public static readonly BinanceEndpoint Trades = new("/api/v3/trades");
    public static readonly BinanceEndpoint Candles = new("/api/v3/klines");
    public static readonly BinanceEndpoint Ticker = new("/api/v3/ticker/24hr");
}