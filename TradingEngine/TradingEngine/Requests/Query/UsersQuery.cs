﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Entities.UserEntity;
using TradingEngine.Entities.WalletEntity;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Requests.Query
{
    public class UsersQuery : IRequest<IEnumerable<UsersQueryResult>>
    {

    }

    public class UsersQueryHandler : IRequestHandler<UsersQuery, IEnumerable<UsersQueryResult>>
    {
        private readonly IUserRepository _userRepository;

        public UsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<IEnumerable<UsersQueryResult>> Handle(UsersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = _userRepository.GetAll(includeProperties: new Expression<Func<User, object>>[] { x => x.Wallets });

            IEnumerable<UsersQueryResult> result = users.Select(x => new UsersQueryResult
            {
                Id = x.Id,
                AccountNumber = x.AccountNumber,
                Name = x.Name,
                Wallets = x.Wallets.Select(y =>
                    new UserWalletQueryResult {
                        Balance = y.Balance,
                        Id = y.Id,
                        WalletId = y.WalletId
                    })
            });

            return Task.FromResult(result);
        }
    }

    public class UsersQueryResult
    {
        public int Id { get; set; }
        public AccountNumber AccountNumber { get; set; }
        public string Name { get; set; }
        public IEnumerable<UserWalletQueryResult> Wallets { get; set; }
    }

    public class UserWalletQueryResult
    {
        public int Id { get; set; }
        public WalletId WalletId { get; set; }
        public Money Balance { get; set; }
    }
}