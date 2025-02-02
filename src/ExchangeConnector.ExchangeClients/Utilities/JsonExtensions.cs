using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExchangeConnector.ExchangeClients.Utilities;

public static class JsonExtensions
{
    public static T FromJson<T>(this string json)
    {
        var result = JsonConvert.DeserializeObject<T>(json);
        if(result is null)
            throw new NullReferenceException("[ExchangeClients.JsonExtensions, method=FromJson] result is null");
        
        return result;
    }
    
    public static decimal ToDecimal(this JToken data)
    {
        return decimal.Parse(data.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    public static DateTimeOffset GetDateTimeFromUTimeMilliseconds(this JToken data)
    {
        var time = long.Parse(data.ToString());
        return DateTimeOffset.FromUnixTimeMilliseconds(time);
    }
}