using System.Threading;
using System.Threading.Tasks;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Entities.CurrencyEntity;
using TradingEngine.Entities.WalletEntity;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Domain.Services
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