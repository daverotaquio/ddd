using System.Collections.Generic;
using TradingEngine.Infrastructure;
using TradingEngine.Models.AccountHistoryEntity;
using TradingEngine.Models.Seedwork;
using TradingEngine.Models.ValueObjects;
using TradingEngine.Models.WalletEntity;

namespace TradingEngine.Models.UserEntity
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
