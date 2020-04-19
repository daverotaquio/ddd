using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Domain.CurrencyDomain;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Domain.WalletDomain;
using TradingEngine.Exceptions;
using TradingEngine.Extensions;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Requests.Commands
{
    [DataContract]
    public class DepositCommand : IRequest<bool>
    {
        public DepositCommand(decimal amount, string currencyKey = CurrencySettings.DefaultCurrencyKey)
        {
            Amount = amount;
            CurrencyKey = string.IsNullOrEmpty(currencyKey) ? CurrencySettings.DefaultCurrencyKey : currencyKey;
        }

        [DataMember]
        public decimal Amount { get; private set; }

        [DataMember]
        public string CurrencyKey { get; private set; }

    }

    public class DepositCommandHandler : IRequestHandler<DepositCommand, bool>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IAuthenticationService _authenticationService;

        public DepositCommandHandler(IWalletRepository walletRepository, ICurrencyRepository currencyRepository, IAuthenticationService authenticationService)
        {
            _walletRepository = walletRepository;
            _currencyRepository = currencyRepository;
            _authenticationService = authenticationService;
        }

        public async Task<bool> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            _authenticationService.CheckLoggedInUser();

            var currency = _currencyRepository.GetAll(x => x.Key == request.CurrencyKey).FirstOrDefault();

            currency.MustNotBeNull(ex: new CurrencyNotFoundException("Currency not found."));

            Wallet wallet = _walletRepository.GetAll(x => x.UserId == Auth.Instance.LoggedInUserId).FirstOrDefault();

            wallet.MustNotBeNull(ex: new WalletNotFoundException(ExceptionMessage.WalletNotFoundExceptionMessage));

            var money = new Money(request.Amount);

            wallet?.Deposit(currency.ExchangeMoney(money), currency);

            _walletRepository.Update(wallet);

            await _walletRepository.SaveChanges(cancellationToken);

            return await Task.FromResult(true);
        }
    }
}
