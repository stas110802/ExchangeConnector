using System.Net.Http.Headers;
using ExchangeConnector.ExchangeClients.Utilities;

namespace ExchangeConnector.ExchangeClients.RestApi;

public class BinanceRequest : BaseRequest
{
    public override BaseRequest Authorize()
    {
        if (Client == null || RequestOptions == null || ApiOptions == null)
            throw new ArgumentException("[Binance Request] You need to create a request first");

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        RequestOptions.Query!.Append($"&timestamp={timestamp}");

        var signature = HashCalculator
            .CalculateHMACSHA256Hash(RequestOptions.BuildQuery(), ApiOptions.SecretKey);
        RequestOptions.Query.Append($"&signature={signature}");

        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Client.DefaultRequestHeaders.Add("X-MBX-APIKEY", ApiOptions.PublicKey);

        return this;
    }
}