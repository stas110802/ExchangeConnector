namespace ExchangeConnector.ExchangeClients.Types;

public class CandleType : BaseType
{
    private CandleType(string value) : base(value) { }

    public static readonly CandleType OneMin = new("1m");
    public static readonly CandleType FiveMin = new("5m");
    public static readonly CandleType FifteenMin = new("15m");
    public static readonly CandleType ThirtyMin = new("30m");
}