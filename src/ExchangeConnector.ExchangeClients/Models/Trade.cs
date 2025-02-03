using ExchangeConnector.ExchangeClients.Types;

namespace ExchangeConnector.ExchangeClients.Models;

public class Trade
{
    /// <summary>
    /// Валютная пара
    /// </summary>
    public string Pair { get; set; }

    /// <summary>
    /// Цена трейда
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Объем трейда
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Направление (buy/sell)
    /// </summary>
    public SideType Side { get; set; }

    /// <summary>
    /// Время трейда
    /// </summary>
    public DateTimeOffset Time { get; set; }
    
    /// <summary>
    /// Id трейда
    /// </summary>
    public string Id { get; set; }

    public override string ToString()
    {
        return $"Валютная пара: {Pair}\n" +
               $"Цена трейда: {Price}\n" +
               $"Объем трейда: {Amount}\n" +
               $"Тип: {Side}\n" +
               $"Время трейда: {Time}\n" +
               $"Id трейда: {Id}";
    }
}