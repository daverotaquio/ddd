using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure.Repositories.Interface;
using TradingEngine.Models.CurrencyEntity;
using TradingEngine.Models.ValueObjects;
using TradingEngine.Models.WalletEntity;

namespace TradingEngine.Requests.Commands
{
    [DataContract]
    public class WithdrawCommand : IRequest<bool>
    {
        public WithdrawCommand(decimal amount, string currencyKey = CurrencySettings.DefaultCurrencyKey)
        {
            Amount = amount;
            CurrencyKey = string.IsNullOrEmpty(currencyKey) ? CurrencySettings.DefaultCurrencyKey : currencyKey;
        }

        [DataMember]
        public decimal Amount { get; private set; }

        [DataMember]
        public string CurrencyKey { get; private set; }
    }

    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, bool>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IAuthenticationService _authenticationService;

        public WithdrawCommandHandler(IWalletRepository walletRepository, ICurrencyRepository currencyRepository, IAuthenticationService authenticationService)
        {
            _walletRepository = walletRepository;
            _currencyRepository = currencyRepository;
            _authenticationService = authenticationService;
        }

        public async Task<bool> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {
            _authenticationService.CheckLoggedInUser();

            Currency currency = _currencyRepository.GetAll(x => x.Key == request.CurrencyKey).FirstOrDefault();

            currency.MustNotBeNull(ex: new CurrencyNotFoundException(ExceptionMessage.CurrencyNotFoundExceptionMessage));

            Wallet wallet = _walletRepository.GetAll(x => x.UserId == Auth.Instance.LoggedInUserId).FirstOrDefault();

            wallet.MustNotBeNull(ex: new WalletNotFoundException(ExceptionMessage.WalletNotFoundExceptionMessage));

            var money = new Money(request.Amount);

            Money amount = currency.ExchangeMoney(money);

            wallet?.Withdraw(amount, currency);

            _walletRepository.Update(wallet);

            await _walletRepository.SaveChanges(cancellationToken);

            return await Task.FromResult(true);
        }
    }
}