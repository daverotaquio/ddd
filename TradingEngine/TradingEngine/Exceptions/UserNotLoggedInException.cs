using System;

namespace TradingEngine.Exceptions
{
    public class UserNotLoggedInException : Exception
    {
        public UserNotLoggedInException(string message) : base(message)
        {
        }
    }
}