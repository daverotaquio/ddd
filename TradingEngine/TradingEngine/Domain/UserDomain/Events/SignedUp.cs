using TradingEngine.Infrastructure;

namespace TradingEngine.Domain.UserDomain.Events
{
    public class SignedUp : Event
    {
        public string Name { get; set; }
    }
}