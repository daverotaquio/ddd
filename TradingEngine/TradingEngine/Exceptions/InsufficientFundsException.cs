using System;

namespace TradingEngine.Exceptions
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(string message)
            : base(message)
        {
        }
    }
}