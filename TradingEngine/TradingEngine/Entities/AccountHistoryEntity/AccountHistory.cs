using System;
using TradingEngine.Entities.UserEntity;
using TradingEngine.Infrastructure;

namespace TradingEngine.Entities.AccountHistoryEntity
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