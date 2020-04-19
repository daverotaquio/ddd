using System;
using TradingEngine.Infrastructure;

namespace TradingEngine.Domain.WalletDomain
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