using TradingEngine.Domain.WalletDomain;
using TradingEngine.Infrastructure.Context;
using TradingEngine.Infrastructure.Repositories.Base;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Infrastructure.Repositories
{
    public class WalletRepository : TradingEngineRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(TradingEngineContext context) : base(context)
        {
        }
    }
}