using TradingEngine.Domain.AccountHistoryDomain;
using TradingEngine.Infrastructure.Context;
using TradingEngine.Infrastructure.Repositories.Base;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Infrastructure.Repositories
{
    public class AccountHistoryRepository : TradingEngineRepository<AccountHistory>, IAccountHistoryRepository
    {
        public AccountHistoryRepository(TradingEngineContext context) : base(context)
        {
        }
    }
}