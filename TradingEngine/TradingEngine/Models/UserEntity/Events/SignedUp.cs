using TradingEngine.Infrastructure;

namespace TradingEngine.Models.UserEntity.Events
{
    public class SignedUp : Event
    {
        public string Name { get; set; }
    }
}