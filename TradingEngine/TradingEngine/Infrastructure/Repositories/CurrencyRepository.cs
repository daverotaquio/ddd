using TradingEngine.Entities.CurrencyEntity;
using TradingEngine.Infrastructure.Context;
using TradingEngine.Infrastructure.Repositories.Base;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Infrastructure.Repositories
{
    public class CurrencyRepository : TradingEngineRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(TradingEngineContext context) : base(context)
        {
        }
    }
}