using TradingEngine.Infrastructure.Context;
using TradingEngine.Infrastructure.Repositories.Base;
using TradingEngine.Infrastructure.Repositories.Interface;
using TradingEngine.Models.CurrencyEntity;

namespace TradingEngine.Infrastructure.Repositories
{
    public class CurrencyRepository : TradingEngineRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(TradingEngineContext context) : base(context)
        {
        }
    }
}