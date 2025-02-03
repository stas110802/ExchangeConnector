using ExchangeConnector.ExchangeClients.Options;

namespace ExchangeConnector.ExchangeClients;

public static class ExchangeApiOptionsFactory
{
    public static ExchangeApiOptions CreateBinanceRestApiOptions()
    {
        return new ExchangeApiOptions
        {
            BaseUri = "https://api.binance.com"
        };
    }
    
    public static ExchangeApiOptions CreateBinanceWebSocketApiOptions()
    {
        return new ExchangeApiOptions
        {
            BaseUri = "wss://stream.binance.com:9443"
        };
    }
}