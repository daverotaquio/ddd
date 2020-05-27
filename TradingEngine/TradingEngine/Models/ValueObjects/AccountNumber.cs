using System;
using TradingEngine.Infrastructure;

namespace TradingEngine.Models.ValueObjects
{
    public class AccountNumber : ValueObject<AccountNumber>
    {
        public Guid Value { get; private set; }

        public AccountNumber(Guid value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}