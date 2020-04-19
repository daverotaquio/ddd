using System.Collections.Generic;
using TradingEngine.Domain.AccountHistoryDomain;
using TradingEngine.Domain.Seedwork;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Domain.WalletDomain;
using TradingEngine.Infrastructure;

namespace TradingEngine.Domain.UserDomain
{
    public class User : Entity, IAggregateRoot
    {
        public virtual ICollection<Wallet> Wallets { get; } = new List<Wallet>();
        public virtual ICollection<AccountHistory> AccountHistory { get; } = new List<AccountHistory>();

        public AccountNumber AccountNumber { get; private set; }

        public string Name { get; private set; }

        public User(AccountNumber accountNumber, string name)
        {
            AccountNumber = accountNumber;
            Name = name;
        }
    }
}
