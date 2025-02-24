using ExchangeConnector.ExchangeClients.Models;
using ExchangeConnector.ExchangeClients.Options;
using ExchangeConnector.ExchangeClients.Types;
using ExchangeConnector.ExchangeClients.Utilities;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace ExchangeConnector.ExchangeClients.Clients.Socket;

public sealed class BinanceSocketClient : ISocketExchangeConnector
{
    private readonly ExchangeApiOptions _options;
    private readonly Dictionary<string, WebSocket> _tradesSocket;
    private readonly Dictionary<string, WebSocket> _candlesSocket;
    
    public BinanceSocketClient(ExchangeApiOptions options)
    {
        _options = options;
        _tradesSocket = new Dictionary<string, WebSocket>();
        _candlesSocket = new Dictionary<string, WebSocket>();
    }
    
    public event Action<Trade>? NewBuyTrade;
    public event Action<Trade>? NewSellTrade;
    
    public void SubscribeTrades(string pair, int maxCount = 100)
    {
        _tradesSocket.Add(pair, new WebSocket(
            $"{_options.BaseUri}/ws/{pair}@trade"));
        _tradesSocket[pair].OnMessage += (_, response) =>
        {
            var data = response.Data.FromJson<JToken>();
            var time = data["T"]
                .GetDateTimeFromUTimeMilliseconds();
            
            var trade = new Trade
            {
                Pair = data["s"].ToString(),
                Price = data["p"].ToObject<decimal>(),
                Amount = data["q"].ToObject<decimal>(),
                Time = time,
                Id = data["t"].ToString()
            };
            
            if (data["m"].ToObject<bool>() == false)
            {
                trade.Side = SideType.Sell;
                NewSellTrade?.Invoke(trade);
            }
            else
            {
                trade.Side = SideType.Buy;
                NewBuyTrade?.Invoke(trade);
            }
        };
        _tradesSocket[pair].Connect();
    }

    public void UnsubscribeTrades(string pair)
    {
        Unsubscribe(_tradesSocket, pair);
    }

    public event Action<Candle>? CandleSeriesProcessing;

    public void SubscribeCandles(string pair, CandleType type,
        DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0)
    {
        _candlesSocket.Add(pair, new WebSocket(
            $"{_options.BaseUri}/ws/{pair}@kline_{type.Value}"));
        
        _candlesSocket[pair].OnMessage += (_, response) =>
        {
            var data = response.Data.FromJson<JToken>()["k"];
            var time = data["t"].GetDateTimeFromUTimeMilliseconds();
            
            var candle = new Candle
            {
                Pair = data["s"].ToString(),
                OpenPrice = data["o"].ToObject<decimal>(),
                HighPrice = data["h"].ToObject<decimal>(),
                LowPrice = data["l"].ToObject<decimal>(),
                ClosePrice = data["c"].ToObject<decimal>(),
                TotalVolume = data["v"].ToObject<decimal>(),
                OpenTime = time
            };
            
            CandleSeriesProcessing?.Invoke(candle);
        };
        _candlesSocket[pair].Connect();
    }

    public void UnsubscribeCandles(string pair)
    {
        Unsubscribe(_candlesSocket, pair);
    }
    
    public void Dispose()
    {
        CloseAllSocket(_tradesSocket);
        CloseAllSocket(_candlesSocket);
    }

    private void Unsubscribe(Dictionary<string, WebSocket> sockets, string key)
    {
        var isSuc = sockets.TryGetValue(key, out var websocket);
        if(isSuc == false) return; 
        
        websocket?.Close();
        sockets.Remove(key);
    }

    private static void CloseAllSocket(Dictionary<string, WebSocket> sockets)
    {
        foreach (var socket in sockets)
            socket.Value.Close();
    }
}