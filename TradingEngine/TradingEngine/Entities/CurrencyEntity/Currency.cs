using TradingEngine.Domain.ValueObjects;
using TradingEngine.Infrastructure;

namespace TradingEngine.Entities.CurrencyEntity
{
    public class Currency : Entity
    {
        public Currency(string key, decimal ratio, string culture)
        {
            Key = key;
            Ratio = ratio;
            Culture = culture;
        }

        public string Key { get; private set; }
        public decimal Ratio { get; private set; }
        public string Culture { get; private set; }

        private Money OriginalValue { get; set; }

        public Money ExchangeMoney(Money money)
        {
            OriginalValue = money;

            decimal newValue = money.Value * Ratio;

            var newMoney = new Money(newValue);

            return newMoney;
        }

        public Money OriginalMoneyValue()
        {
            return OriginalValue ?? new Money(0);
        }
    }
}