using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
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
    [DataContract]
    public class UserLoginQuery : IRequest<UserLoginQueryResult>
    {
        public UserLoginQuery(string name)
        {
            Name = name;
        }

        [DataMember]
        public string Name { get; private set; }
    }

    public class UserLoginQueryHandler : IRequestHandler<UserLoginQuery, UserLoginQueryResult>
    {
        private readonly IUserRepository _userRepository;

        public UserLoginQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<UserLoginQueryResult> Handle(UserLoginQuery request, CancellationToken cancellationToken)
        {
            User user = _userRepository.GetAll(x => x.Name.Equals(request.Name), includeProperties: new Expression<Func<User, object>>[] { x => x.Wallets }).FirstOrDefault();

            user.MustNotBeNull(ex: new UserNotFoundException(ExceptionMessage.UserNotFoundExceptionMessage));

            var result = new UserLoginQueryResult
            {
                Name = user.Name,
                Id = user.Id,
                AccountNumber = user.AccountNumber,
                Wallets = user.Wallets.Select(x => new UserLoginWalletQueryResult
                {
                    Id = x.Id,
                    WalletId = x.WalletId,
                    Balance = x.Balance
                })
            };

            return Task.FromResult(result);
        }
    }

    public class UserLoginQueryResult
    {
        public int Id { get; set; }
        public AccountNumber AccountNumber { get; set; }
        public string Name { get; set; }
        public IEnumerable<UserLoginWalletQueryResult> Wallets { get; set; }
    }

    public class UserLoginWalletQueryResult
    {
        public int Id { get; set; }
        public WalletId WalletId { get; set; }
        public Money Balance { get; set; }
    }
}