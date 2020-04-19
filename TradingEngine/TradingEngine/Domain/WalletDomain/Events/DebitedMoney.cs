using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Domain.AccountHistoryDomain;
using TradingEngine.Domain.CurrencyDomain;
using TradingEngine.Domain.UserDomain;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Domain.WalletDomain.Events
{
    public class DebitedMoney : Event
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public Currency Currency { get; set; }
    }

    public class DebitedMoneyEventHandler : INotificationHandler<DebitedMoney>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IAccountHistoryRepository _accountHistoryRepository;
        private readonly IUserRepository _userRepository;

        public DebitedMoneyEventHandler(IWalletRepository walletRepository, IAccountHistoryRepository accountHistoryRepository, IUserRepository userRepository)
        {
            _walletRepository = walletRepository;
            _accountHistoryRepository = accountHistoryRepository;
            _userRepository = userRepository;
        }

        public async Task Handle(DebitedMoney notification, CancellationToken cancellationToken)
        {
            Wallet wallet = _walletRepository.GetById(notification.EntityId);

            User toUser = _userRepository.GetById(notification.ToUserId);

            wallet.MustNotBeNull(ex: new WalletNotFoundException(ExceptionMessage.WalletNotFoundExceptionMessage));
            toUser.MustNotBeNull(ex: new UserNotFoundException(ExceptionMessage.UserNotFoundExceptionMessage));

            var debitedMoney = new Money(notification.Amount);
            Money originalDepositedMoney = notification.Currency.OriginalMoneyValue();

            var accountHistory = new AccountHistory(
                notification.FromUserId,
                DateTime.Now,
                notification.Currency.Key == CurrencySettings.DefaultCurrencyKey ?
                    $"{debitedMoney} has been debited from your account to {toUser.Name}. New account balance is {wallet.Balance}." :
                    $"{notification.Currency.Key} {originalDepositedMoney.Value:#.00} or {debitedMoney} has been debited from your account to {toUser.Name}. New account balance is {wallet.Balance}."
            );

            _accountHistoryRepository.Add(accountHistory);

            await _accountHistoryRepository.SaveChanges(cancellationToken);
        }
    }
}