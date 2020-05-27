using System;
using TradingEngine.Infrastructure;

namespace TradingEngine.Models.WalletEntity
{
    public class WalletId : ValueObject<WalletId>
    {
        public Guid Value { get; }

        public WalletId(Guid value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}