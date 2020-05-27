using System.Threading;
using System.Threading.Tasks;
using TradingEngine.Infrastructure.Repositories.Interface;
using TradingEngine.Models.CurrencyEntity;
using TradingEngine.Models.ValueObjects;
using TradingEngine.Models.WalletEntity;

namespace TradingEngine.Models.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IWalletRepository _walletRepository;

        public TransactionService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task TransferFunds(Wallet fromWallet, Wallet toWallet, Money amount, Currency currency, CancellationToken cancellationToken)
        {
            fromWallet.Debit(toWallet.UserId, amount, currency);
            toWallet.Credit(fromWallet.UserId, amount, currency);

            _walletRepository.Update(fromWallet);
            _walletRepository.Update(toWallet);

            await _walletRepository.SaveChanges(cancellationToken);
        }
    }
}