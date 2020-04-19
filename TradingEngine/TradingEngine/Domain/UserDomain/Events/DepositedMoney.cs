using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Domain.AccountHistoryDomain;
using TradingEngine.Domain.CurrencyDomain;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Domain.WalletDomain;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Domain.UserDomain.Events
{
    public class DepositedMoney : Event
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public Currency Currency { get; set; }
    }

    public class DepositedMoneyEventHandler : INotificationHandler<DepositedMoney>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IAccountHistoryRepository _accountHistoryRepository;
        private readonly IUserRepository _userRepository;

        public DepositedMoneyEventHandler(IWalletRepository walletRepository, IAccountHistoryRepository accountHistoryRepository, IUserRepository userRepository)
        {
            _walletRepository = walletRepository;
            _accountHistoryRepository = accountHistoryRepository;
            _userRepository = userRepository;
        }

        public Task Handle(DepositedMoney notification, CancellationToken cancellationToken)
        {
            Wallet wallet = _walletRepository.GetById(notification.EntityId);

            User user = _userRepository.GetById(notification.UserId);

            wallet.MustNotBeNull(ex: new WalletNotFoundException(ExceptionMessage.WalletNotFoundExceptionMessage));
            user.MustNotBeNull(ex: new UserNotFoundException(ExceptionMessage.UserNotFoundExceptionMessage));

            var depositedMoney = new Money(notification.Amount);
            Money originalDepositedMoney = notification.Currency.OriginalMoneyValue();

            var accountHistory = new AccountHistory(
                notification.UserId,
                DateTime.Now,
                notification.Currency.Key == CurrencySettings.DefaultCurrencyKey ?
                    $"{user.Name} deposited {depositedMoney}. Total account balance is {wallet.Balance}." :
                    $"{user.Name} deposited {notification.Currency.Key} {originalDepositedMoney.Value:#.00} or {depositedMoney}. Total account balance is {wallet.Balance}."
                );

            _accountHistoryRepository.Add(accountHistory);

            _accountHistoryRepository.SaveChanges(cancellationToken);

            return Task.CompletedTask;
        }
    }
}