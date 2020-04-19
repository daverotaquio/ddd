namespace TradingEngine.Constants
{
    public static class ExceptionMessage
    {
        public const string UserNotFoundExceptionMessage = "User not found.";
        public const string UserNotLoggedInExceptionMessage = "User not logged in.";
        public const string WalletNotFoundExceptionMessage = "Wallet not found.";
        public const string InsufficientFundsExceptionMessage = "Not enough funds. Cannot withdraw an amount greater than your current balance.";
        public const string CurrencyNotFoundExceptionMessage = "Currency not found.";
    }
}