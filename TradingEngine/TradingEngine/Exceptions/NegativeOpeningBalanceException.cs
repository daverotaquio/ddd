using System;

namespace TradingEngine.Exceptions
{
    public class NegativeOpeningBalanceException : Exception
    {
        public NegativeOpeningBalanceException(string message) : base(message)
        {
        }
    }
}