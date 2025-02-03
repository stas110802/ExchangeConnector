using ExchangeConnector.ExchangeClients;
using ExchangeConnector.ExchangeClients.Clients.Rest;

namespace ExchangeConnector.UI.MVVM.ViewModels;

public class MainWindowVM : ObservableObject
{
    private readonly BinanceRestClient _binanceRestClient;
    private decimal _btcTotalBalance;
    private decimal _usdtTotalBalance;
    private decimal _xrpTotalBalance;
    private decimal _xmrTotalBalance;
    private decimal _dashTotalBalance;

    private readonly Dictionary<string, decimal> _balance = new()
    {
        ["BTC"] = 1,
        ["XRP"] = 15000,
        ["XMR"] = 50,// Дохлая монета
        ["DASH"] = 30
    };

    public MainWindowVM()
    {
        var options = ExchangeApiOptionsFactory.CreateBinanceRestApiOptions();
        _binanceRestClient = new BinanceRestClient(options);
        RefreshBalanceCommand = new AsyncCommand(InitBalance);
    }

    public decimal BtcTotalBalance
    {
        get => _btcTotalBalance;
        set => Set(ref _btcTotalBalance, value);
    }

    public decimal UsdtTotalBalance
    {
        get => _usdtTotalBalance;
        set => Set(ref _usdtTotalBalance, value);
    }

    public decimal XrpTotalBalance
    {
        get => _xrpTotalBalance;
        set => Set(ref _xrpTotalBalance, value);
    }

    public decimal XmrTotalBalance
    {
        get => _xmrTotalBalance;
        set => Set(ref _xmrTotalBalance, value);
    }

    public decimal DashTotalBalance
    {
        get => _dashTotalBalance;
        set => Set(ref _dashTotalBalance, value);
    }

    public AsyncCommand RefreshBalanceCommand { get; init; }

    private async Task InitBalance()
    {
        BtcTotalBalance = await GetTotalBalance("BTC");
        UsdtTotalBalance = await GetTotalBalance("USDT");
        XrpTotalBalance = await GetTotalBalance("XRP");
        XmrTotalBalance = await GetTotalBalance("XMR");
        DashTotalBalance = await GetTotalBalance("DASH");
    }

    private async Task<decimal> GetTotalBalance(string secondCoin)
    {
        decimal value = 0;
        foreach (var firstCoin in _balance)
        {
            if (firstCoin.Key == secondCoin)
            {
                value += firstCoin.Value;
                continue;
            }

            var currency =  firstCoin.Key + secondCoin;
            var ticker = await _binanceRestClient.GetTickerAsync(currency);
            
            if (ticker.LastPrice == 0)
            {
                currency = secondCoin + firstCoin.Key;
                ticker = await _binanceRestClient.GetTickerAsync(currency);
                if(ticker.LastPrice != 0)
                    value += firstCoin.Value / ticker.LastPrice;
                continue;
            }
            value += ticker.LastPrice * firstCoin.Value;
        }

        return value;
    }
}