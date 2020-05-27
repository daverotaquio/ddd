using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure.Repositories.Interface;
using TradingEngine.Models.UserEntity;
using TradingEngine.Models.ValueObjects;
using TradingEngine.Models.WalletEntity;

namespace TradingEngine.Requests.Query
{
    public class CheckBalanceQuery : IRequest<CheckBalanceQueryResult>
    {
    }

    public class CheckBalanceQueryHandler : IRequestHandler<CheckBalanceQuery, CheckBalanceQueryResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletRepository _walletRepository;

        public CheckBalanceQueryHandler(IUserRepository userRepository, IWalletRepository walletRepository)
        {
            _userRepository = userRepository;
            _walletRepository = walletRepository;
        }

        public Task<CheckBalanceQueryResult> Handle(CheckBalanceQuery request, CancellationToken cancellationToken)
        {
            Auth.Instance.LoggedInUserId.MustNotBeDefault(ex: new UserNotLoggedInException(ExceptionMessage.UserNotLoggedInExceptionMessage));

            User user = _userRepository.GetById(Auth.Instance.LoggedInUserId);

            user.MustNotBeNull(ex: new UserNotFoundException(ExceptionMessage.UserNotFoundExceptionMessage));

            List<Wallet> wallet = _walletRepository.GetAll(x => x.UserId == Auth.Instance.LoggedInUserId).ToList();

            wallet.Count.MustBeGreaterThan0(ex: new WalletNotFoundException(ExceptionMessage.WalletNotFoundExceptionMessage));

            var totalBalance = new Money(wallet.Select(x => x.Balance.Value).Sum());

            var result = new CheckBalanceQueryResult
            {
                Message = $"Hi {user.Name}! Your balance as of {DateTime.Now:f} is {totalBalance}"
            };

            return Task.FromResult(result);
        }
    }

    public class CheckBalanceQueryResult
    {
        public string Message { get; set; }
    }
}