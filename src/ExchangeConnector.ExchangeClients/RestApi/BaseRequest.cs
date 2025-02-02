using ExchangeConnector.ExchangeClients.Options;
using ExchangeConnector.ExchangeClients.Types;

namespace ExchangeConnector.ExchangeClients.RestApi;

public abstract class BaseRequest
{
    public HttpClient? Client;
    public RequestOptions? RequestOptions { get; set; }
    public ExchangeApiOptions? ApiOptions { get; set; }
    public abstract BaseRequest Authorize();

    public async Task<string> ExecuteAsync()
    {
        if (RequestOptions == null || Client == null)
            throw new NullReferenceException("[Request error] : First you need to execute 'Create' method.");

        HttpResponseMessage? response = null;
        
        if(RequestOptions.Type == MethodType.Get)
            response = await Client.GetAsync(RequestOptions.FullPath);
        if(RequestOptions.Type == MethodType.Post)
            response = await Client.PostAsync(RequestOptions.FullPath, null);
        
        response!.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        return responseBody;
    }
}