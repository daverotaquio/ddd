using TradingEngine.Infrastructure.Context;
using TradingEngine.Infrastructure.Repositories.Base;
using TradingEngine.Infrastructure.Repositories.Interface;
using TradingEngine.Models.UserEntity;

namespace TradingEngine.Infrastructure.Repositories
{
    public class UserRepository : TradingEngineRepository<User>, IUserRepository
    {
        public UserRepository(TradingEngineContext context) : base(context)
        {
        }
    }
}