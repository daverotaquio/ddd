using System;
using TradingEngine.Constants;
using TradingEngine.Exceptions;

namespace TradingEngine
{
    public sealed class Auth
    {
        private Auth()
        {
        }

        private static readonly Lazy<Auth> Lazy = new Lazy<Auth>(() => new Auth());
        public static Auth Instance => Lazy.Value;

        public int LoggedInUserId { get; set; }
    }

    public interface IAuthenticationService
    {
        void CheckLoggedInUser();
    }

    public class AuthenticationService : IAuthenticationService
    {
        public void CheckLoggedInUser()
        {
            Auth.Instance.LoggedInUserId.MustNotBeDefault(ex: new UserNotLoggedInException(ExceptionMessage.UserNotLoggedInExceptionMessage));
        }
    }
}