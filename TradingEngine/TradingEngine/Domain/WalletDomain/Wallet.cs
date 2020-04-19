using System;
using TradingEngine.Constants;
using TradingEngine.Domain.CurrencyDomain;
using TradingEngine.Domain.Seedwork;
using TradingEngine.Domain.UserDomain;
using TradingEngine.Domain.UserDomain.Events;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Domain.WalletDomain.Events;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure;

namespace TradingEngine.Domain.WalletDomain
{
    public class Wallet : Entity, IAggregateRoot
    {
        public WalletId WalletId { get; private set; }

        public Money Balance { get; private set; }
        public int UserId { get; private set; }

        public User User { get; set; }

        public Wallet(int id, WalletId walletId, Money balance, int userId)
        {
            Id = id;
            WalletId = walletId;
            Balance = balance;
            UserId = userId;

            if (Id == 0) WalletCreatedEvent(balance);
        }

        private void WalletCreatedEvent(Money balance)
        {
            AddDomainEvent(new WalletCreated
            {
                UserId = UserId,
                InitialBalance = balance
            });
        }

        public virtual void Deposit(Money amount, Currency currency = null)
        {
            if (amount.Value <= 0)
                throw new InvalidOperationException("Cannot deposit an amount less than or equal to zero.");

            Balance = Balance.Add(amount);

            var depositMoney =  new DepositedMoney
            {
                EntityId = Id,
                Amount = amount.Value,
                UserId = UserId,
                Currency = currency
            };

            AddDomainEvent(depositMoney);
        }

        public void Withdraw(Money amount, Currency currency = null)
        {
            if(amount.Value < 0)
                throw new InvalidOperationException("Cannot withdraw an amount less than or equal to zero.");

            decimal preCalculatedBalance = Balance.Value - amount.Value;

            preCalculatedBalance.Must(x => x >= 0, ex: new InsufficientFundsException(ExceptionMessage.InsufficientFundsExceptionMessage));

            Balance = Balance.Subtract(amount);

            var withdrawMoney =  new WithdrewMoney
            {
                EntityId = Id,
                Amount = amount.Value,
                UserId = UserId,
                Currency = currency
            };

            AddDomainEvent(withdrawMoney);
        }
        public void Credit(int fromUserId, Money amount, Currency currency = null)
        {
            if (amount.Value <= 0)
                throw new InvalidOperationException("Cannot credit an amount less than or equal to zero.");

            Balance = Balance.Add(amount);

            var creditedMoney =  new CreditedMoney
            {
                EntityId = Id,
                Amount = amount.Value,
                ToUserId = UserId,
                FromUserId = fromUserId,
                Currency = currency
            };

            AddDomainEvent(creditedMoney);
        }

        public void Debit(int toUserId, Money amount, Currency currency = null)
        {
            if(amount.Value < 0)
                throw new InvalidOperationException("Cannot debit an amount less than or equal to zero.");

            decimal preCalculatedBalance = Balance.Value - amount.Value;

            preCalculatedBalance.Must(x => x >= 0, ex: new InsufficientFundsException(ExceptionMessage.InsufficientFundsExceptionMessage));

            Balance = Balance.Subtract(amount);

            var debitedMoney =  new DebitedMoney
            {
                EntityId = Id,
                Amount = amount.Value,
                FromUserId = UserId,
                ToUserId = toUserId,
                Currency = currency
            };

            AddDomainEvent(debitedMoney);
        }
    }
}