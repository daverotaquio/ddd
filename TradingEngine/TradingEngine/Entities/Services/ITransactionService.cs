using System.Threading;
using System.Threading.Tasks;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Entities.CurrencyEntity;
using TradingEngine.Entities.WalletEntity;

namespace TradingEngine.Domain.Services
{
    public interface ITransactionService
    {
        Task TransferFunds(Wallet fromWallet, Wallet toWallet, Money amount, Currency currency, CancellationToken cancellationToken);
    }
}