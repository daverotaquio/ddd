using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Constants;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Entities.WalletEntity;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Requests.Commands
{
    [DataContract]
    public class AddWalletCommand : IRequest<bool>
    {
        public AddWalletCommand(decimal initialBalance)
        {
            InitialBalance = initialBalance;
        }

        [DataMember]
        public decimal InitialBalance { get; private set; }
    }

    public class AddWalletCommandHandler : IRequestHandler<AddWalletCommand, bool>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IAuthenticationService _authenticationService;

        public AddWalletCommandHandler(IWalletRepository walletRepository, IAuthenticationService authenticationService)
        {
            _walletRepository = walletRepository;
            _authenticationService = authenticationService;
        }

        public async Task<bool> Handle(AddWalletCommand request, CancellationToken cancellationToken)
        {
            _authenticationService.CheckLoggedInUser();

            var newWallet = new Wallet(0, new WalletId(Guid.NewGuid()), new Money(request.InitialBalance), Auth.Instance.LoggedInUserId);

            _walletRepository.Add(newWallet);

            await _walletRepository.SaveChanges(cancellationToken);

            return await Task.FromResult(true);
        }
    }
}