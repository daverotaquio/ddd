using MediatR;

namespace TradingEngine.Infrastructure
{
    public class Event : INotification
    {
        public int EntityId { get; set; }
    }
}