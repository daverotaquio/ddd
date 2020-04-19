using System;
using TradingEngine.Domain.UserDomain;
using TradingEngine.Infrastructure;

namespace TradingEngine.Domain.AccountHistoryDomain
{
    public class AccountHistory : Entity
    {
        public AccountHistory(int userId, DateTime dateCreated, string message)
        {
            Message = message;
            DateCreated = dateCreated;
            UserId = userId;
        }

        public string Message { get; private set; }
        public DateTime DateCreated { get; private set; }
        public int UserId { get; private set; }
        public User User { get; set; }
    }
}