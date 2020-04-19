using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Domain.AccountHistoryDomain;
using TradingEngine.Domain.UserDomain;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Domain.WalletDomain.Events
{
    public class WalletCreated : Event
    {
        public int UserId { get; set; }
        public Money InitialBalance { get; set; }
    }

    public class WalletCreatedEventHandler : INotificationHandler<WalletCreated>
    {
        private readonly IAccountHistoryRepository _accountHistoryRepository;
        private readonly IUserRepository _userRepository;

        public WalletCreatedEventHandler(IAccountHistoryRepository accountHistoryRepository, IUserRepository userRepository)
        {
            _accountHistoryRepository = accountHistoryRepository;
            _userRepository = userRepository;
        }

        public async Task Handle(WalletCreated notification, CancellationToken cancellationToken)
        {
            User user = _userRepository.GetById(notification.UserId);

            user.MustNotBeNull(ex: new UserNotFoundException(ExceptionMessage.UserNotFoundExceptionMessage));

            var accountHistory = new AccountHistory(
                notification.UserId,
                DateTime.Now,
                $"New wallet created for {user.Name} with an initial balance of {notification.InitialBalance}."
            );

            _accountHistoryRepository.Add(accountHistory);

            await _accountHistoryRepository.SaveChanges(cancellationToken);
        }
    }
}