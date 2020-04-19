using System.Threading;
using System.Threading.Tasks;
using TradingEngine.Domain.CurrencyDomain;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Domain.WalletDomain;

namespace TradingEngine.Domain.Services
{
    public interface ITransactionService
    {
        Task TransferFunds(Wallet fromWallet, Wallet toWallet, Money amount, Currency currency, CancellationToken cancellationToken);
    }
}