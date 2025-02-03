namespace ExchangeConnector.ExchangeClients.Models;

public class Ticker
{
    public string Symbol { get; set; }
    public decimal LastPrice { get; set; }
    public decimal HighPrice { get; set; }
    public decimal LowPrice { get; set; }
    public decimal Volume { get; set; }
}