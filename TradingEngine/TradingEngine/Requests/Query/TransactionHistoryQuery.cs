using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Entities.AccountHistoryEntity;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Requests.Query
{
    public class TransactionHistoryQuery : IRequest<IEnumerable<TransactionHistoryQueryResult>>
    {
    }

    public class TransactionHistoryQueryHandler : IRequestHandler<TransactionHistoryQuery, IEnumerable<TransactionHistoryQueryResult>>
    {
        private readonly IAccountHistoryRepository _accountHistoryRepository;

        public TransactionHistoryQueryHandler(IAccountHistoryRepository accountHistoryRepository)
        {
            _accountHistoryRepository = accountHistoryRepository;
        }

        public Task<IEnumerable<TransactionHistoryQueryResult>> Handle(TransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            Auth.Instance.LoggedInUserId.MustNotBeDefault(ex: new UserNotLoggedInException(ExceptionMessage.UserNotLoggedInExceptionMessage));

            IEnumerable<AccountHistory> accountHistory = _accountHistoryRepository.GetAll(x => x.UserId == Auth.Instance.LoggedInUserId);

            IEnumerable<TransactionHistoryQueryResult> result = accountHistory.Select(x => new TransactionHistoryQueryResult
            {
                DateCreated = x.DateCreated.ToString("f"),
                Message = x.Message
            });

            return Task.FromResult(result);
        }
    }

    public class TransactionHistoryQueryResult
    {
        public string Message { get; set; }
        public string DateCreated { get; set; }
    }
}