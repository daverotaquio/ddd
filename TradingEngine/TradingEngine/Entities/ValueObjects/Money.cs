using System.Globalization;
using TradingEngine.Infrastructure;

namespace TradingEngine.Domain.ValueObjects
{
    public class Money : ValueObject<Money>
    {
        public static Money Default = new Money(0M);

        public decimal Value { get; }


        public Money(decimal value)
        {
            Value = value;
        }

        public Money Add(Money amount)
        {
            decimal newAmount = Value + amount.Value;
            return new Money(newAmount);
        }

        public Money Subtract(Money amount)
        {
            decimal newAmount = Value - amount.Value;
            return new Money(newAmount);
        }

        public static bool operator >(Money left, Money right)
        {
            return left.Value > right.Value;
        }

        public static bool operator <(Money left, Money right)
        {
            return left.Value < right.Value;
        }

        public override string ToString()
        {
            return Value.ToString("C", new CultureInfo("en-PH"));
        }
    }
}