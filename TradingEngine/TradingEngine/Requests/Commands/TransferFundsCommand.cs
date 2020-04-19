using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Domain.CurrencyDomain;
using TradingEngine.Domain.Services;
using TradingEngine.Domain.UserDomain;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Domain.WalletDomain;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Requests.Commands
{
    [DataContract]
    public class TransferFundsCommand : IRequest<bool>
    {
        public TransferFundsCommand(Guid toWalletId, decimal amount, string currencyKey = CurrencySettings.DefaultCurrencyKey)
        {
            ToWalletId = toWalletId;
            Amount = amount;
            CurrencyKey = string.IsNullOrEmpty(currencyKey) ? CurrencySettings.DefaultCurrencyKey : currencyKey;
        }

        [DataMember]
        public Guid ToWalletId { get; private set; }
        [DataMember]
        public decimal Amount { get; private set; }
        [DataMember]
        public string CurrencyKey { get; private set; }
    }

    public class TransferFundsCommandHandler : IRequestHandler<TransferFundsCommand, bool>
    {
        private readonly ITransactionService _transactionService;
        private readonly IUserRepository _userRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IAuthenticationService _authenticationService;

        public TransferFundsCommandHandler(ITransactionService transactionService, IUserRepository userRepository, IWalletRepository walletRepository, ICurrencyRepository currencyRepository, IAuthenticationService authenticationService)
        {
            _transactionService = transactionService;
            _userRepository = userRepository;
            _walletRepository = walletRepository;
            _currencyRepository = currencyRepository;
            _authenticationService = authenticationService;
        }

        public async Task<bool> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
        {
            _authenticationService.CheckLoggedInUser();

            User loggedInUser = _userRepository.GetById(Auth.Instance.LoggedInUserId);
            loggedInUser.MustNotBeNull(ex: new UserNotFoundException(ExceptionMessage.UserNotFoundExceptionMessage));

            Wallet fromWallet = _walletRepository.GetAll(x => x.UserId == Auth.Instance.LoggedInUserId).FirstOrDefault();
            fromWallet.MustNotBeNull(ex: new WalletNotFoundException(ExceptionMessage.WalletNotFoundExceptionMessage));

            Wallet toWallet = _walletRepository.GetAll(x => x.WalletId.Value == request.ToWalletId).FirstOrDefault();
            toWallet.MustNotBeNull(ex: new WalletNotFoundException(ExceptionMessage.WalletNotFoundExceptionMessage));

            fromWallet.WalletId.Value.Must(x => x != toWallet.WalletId.Value, ex: new InvalidOperationException("Cannot transfer on the same wallet."));

            Currency currency = _currencyRepository.GetAll(x => x.Key == request.CurrencyKey).FirstOrDefault();
            currency.MustNotBeNull(ex: new CurrencyNotFoundException("Currency not found."));

            var money = new Money(request.Amount);

            await _transactionService.TransferFunds(fromWallet, toWallet, currency.ExchangeMoney(money), currency, cancellationToken);

            return true;
        }
    }
}