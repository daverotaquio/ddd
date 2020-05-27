using System.Threading;
using System.Threading.Tasks;
using TradingEngine.Models.CurrencyEntity;
using TradingEngine.Models.ValueObjects;
using TradingEngine.Models.WalletEntity;

namespace TradingEngine.Models.Services
{
    public interface ITransactionService
    {
        Task TransferFunds(Wallet fromWallet, Wallet toWallet, Money amount, Currency currency, CancellationToken cancellationToken);
    }
}