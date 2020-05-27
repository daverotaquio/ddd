using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Entities.AccountHistoryEntity;
using TradingEngine.Entities.CurrencyEntity;
using TradingEngine.Entities.WalletEntity;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Entities.UserEntity.Events
{
    public class WithdrewMoney : Event
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public Currency Currency { get; set; }
    }

    public class WithdrewMoneyEventHandler : INotificationHandler<WithdrewMoney>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IAccountHistoryRepository _accountHistoryRepository;
        private readonly IUserRepository _userRepository;

        public WithdrewMoneyEventHandler(IWalletRepository walletRepository, IAccountHistoryRepository accountHistoryRepository, IUserRepository userRepository)
        {
            _walletRepository = walletRepository;
            _accountHistoryRepository = accountHistoryRepository;
            _userRepository = userRepository;
        }

        public async Task Handle(WithdrewMoney notification, CancellationToken cancellationToken)
        {
            Wallet wallet = _walletRepository.GetById(notification.EntityId);

            User user = _userRepository.GetById(notification.UserId);

            wallet.MustNotBeNull(ex: new WalletNotFoundException("Wallet not found."));
            user.MustNotBeNull(ex: new UserNotFoundException(ExceptionMessage.UserNotFoundExceptionMessage));

            var withdrawnMoney = new Money(notification.Amount);
            Money originalWithdrawnMoney = notification.Currency.OriginalMoneyValue();

            var accountHistory = new AccountHistory(
                notification.UserId,
                DateTime.Now,
                notification.Currency.Key == CurrencySettings.DefaultCurrencyKey ?
                    $"{user.Name} has withdrawn {withdrawnMoney}. Total account balance is {wallet.Balance}." :
                    $"{user.Name} has withdrawn {notification.Currency.Key} {originalWithdrawnMoney.Value:#.00} or {withdrawnMoney}. Total account balance is {wallet.Balance}."
            );

            _accountHistoryRepository.Add(accountHistory);

            await _accountHistoryRepository.SaveChanges(cancellationToken);
        }
    }
}