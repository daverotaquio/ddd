using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TradingEngine.Constants;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure.Repositories.Interface;
using TradingEngine.Models.CurrencyEntity;
using TradingEngine.Models.Services;
using TradingEngine.Models.UserEntity;
using TradingEngine.Models.WalletEntity;
using TradingEngine.Requests.Commands;

namespace TradingEngine.Tests.CommandHandlers
{
    public class TransferFundsCommandHandlerTests
    {
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;

        private TransferFundsCommand _transferFundsCommand;
        private User _user;
        private IEnumerable<Wallet> _fromWallet;
        private IEnumerable<Wallet> _toWallet;

        private TransferFundsCommandHandler _target;


        public TransferFundsCommandHandlerTests()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
        }

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();

            _transferFundsCommand = fixture.Build<TransferFundsCommand>()
                .Create();

            _user = fixture.Build<User>()
                .Create();

            _fromWallet = fixture.Build<Wallet>()
                .CreateMany(1);

            _toWallet = fixture.Build<Wallet>()
                .CreateMany(1);

            _target = new TransferFundsCommandHandler(_transactionServiceMock.Object, _userRepositoryMock.Object, _walletRepositoryMock.Object,
                _currencyRepositoryMock.Object, _authenticationServiceMock.Object);
        }

        [TestCase]
        public void WhenUserIsNotLoggedIn_ThenTransferFundsCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock.Setup(x => x.CheckLoggedInUser())
                .Throws(new UserNotLoggedInException(ExceptionMessage.UserNotFoundExceptionMessage));

            _target.Invoking(async x => await x.Handle(_transferFundsCommand, CancellationToken.None)).Should().Throw<UserNotLoggedInException>();
        }

        [TestCase]
        public void WhenUserIsNull_ThenTransferFundsCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock.Setup(x => x.CheckLoggedInUser());

            _userRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()));

            _target.Invoking(async x => await x.Handle(_transferFundsCommand, CancellationToken.None)).Should().Throw<UserNotFoundException>();
        }

        [TestCase]
        public void WhenWalletsAreNull_ThenTransferFundsCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock.Setup(x => x.CheckLoggedInUser());

            _userRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(_user);

            _walletRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Wallet, bool>>>(), null, false));

            _walletRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Wallet, bool>>>(), null, false));

            _target.Invoking(async x => await x.Handle(_transferFundsCommand, CancellationToken.None)).Should().Throw<WalletNotFoundException>();
        }

        [TestCase]
        public void WhenCurrencyIsNull_ThenTransferFundsCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock.Setup(x => x.CheckLoggedInUser());

            _userRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(_user);

            _walletRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Wallet, bool>>>(), null, false))
                .Returns(_toWallet);

            _walletRepositoryMock.Setup(x => x.GetAll(y => y.UserId == Auth.Instance.LoggedInUserId, null, false))
                .Returns(_fromWallet);

            _currencyRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Currency, bool>>>(), null, false));

            _target.Invoking(async x => await x.Handle(_transferFundsCommand, CancellationToken.None)).Should().Throw<CurrencyNotFoundException>();
        }
    }
}