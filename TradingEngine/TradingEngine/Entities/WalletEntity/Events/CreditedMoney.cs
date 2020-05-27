using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Entities.AccountHistoryEntity;
using TradingEngine.Entities.CurrencyEntity;
using TradingEngine.Entities.UserEntity;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Entities.WalletEntity.Events
{
    public class CreditedMoney : Event
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int ToUserId { get; set; }
        public int FromUserId { get; set; }
        public Currency Currency { get; set; }
    }

    public class CreditedMoneyEventHandler : INotificationHandler<CreditedMoney>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IAccountHistoryRepository _accountHistoryRepository;
        private readonly IUserRepository _userRepository;

        public CreditedMoneyEventHandler(IWalletRepository walletRepository, IAccountHistoryRepository accountHistoryRepository, IUserRepository userRepository)
        {
            _walletRepository = walletRepository;
            _accountHistoryRepository = accountHistoryRepository;
            _userRepository = userRepository;
        }

        public async Task Handle(CreditedMoney notification, CancellationToken cancellationToken)
        {
            Wallet wallet = _walletRepository.GetById(notification.EntityId);

            User fromUser = _userRepository.GetById(notification.FromUserId);

            wallet.MustNotBeNull(ex: new WalletNotFoundException(ExceptionMessage.WalletNotFoundExceptionMessage));
            fromUser.MustNotBeNull(ex: new UserNotFoundException(ExceptionMessage.UserNotFoundExceptionMessage));

            var creditedMoney = new Money(notification.Amount);
            Money originalDepositedMoney = notification.Currency.OriginalMoneyValue();

            var accountHistory = new AccountHistory(
                notification.ToUserId,
                DateTime.Now,
                notification.Currency.Key == CurrencySettings.DefaultCurrencyKey ?
                    $"{creditedMoney} has been credited to your account from {fromUser.Name}. New account balance is {wallet.Balance}." :
                    $"{notification.Currency.Key} {originalDepositedMoney.Value:#.00} or {creditedMoney} has been credited to your account from {fromUser.Name}. New account balance is {wallet.Balance}."
            );

            _accountHistoryRepository.Add(accountHistory);

            await _accountHistoryRepository.SaveChanges(cancellationToken);
        }
    }
}