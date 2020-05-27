using TradingEngine.Infrastructure;

namespace TradingEngine.Entities.UserEntity.Events
{
    public class SignedUp : Event
    {
        public string Name { get; set; }
    }
}