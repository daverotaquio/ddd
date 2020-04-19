using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TradingEngine.Constants;
using TradingEngine.Domain.CurrencyDomain;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Domain.WalletDomain;
using TradingEngine.Exceptions;
using TradingEngine.Extensions;
using TradingEngine.Infrastructure.Repositories.Interface;
using TradingEngine.Requests.Commands;

namespace TradingEngine.Tests.CommandHandlers
{
    public class DepositCommandHandlerTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;

        private DepositCommand _depositCommand;
        private DepositCommand _depositCommandWithZeroMoney;

        private IEnumerable<Currency> _currencies;
        private IEnumerable<Wallet> _wallets;

        private DepositCommandHandler _target;

        public DepositCommandHandlerTests()
        {
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _walletRepositoryMock = new Mock<IWalletRepository>();
        }

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();

            _depositCommand = fixture.Build<DepositCommand>()
                .FromFactory(() => new DepositCommand(1, CurrencySettings.DefaultCurrencyKey))
                .Create();

            _depositCommandWithZeroMoney = fixture.Build<DepositCommand>()
                .FromFactory(() => new DepositCommand(0, CurrencySettings.DefaultCurrencyKey))
                .Create();

            _currencies = fixture.Build<Currency>()
                //.FromFactory(() => new Currency("PHP", 1,"en-PH"))
                .CreateMany(5);

            _wallets = fixture.Build<Wallet>()
                .CreateMany(1);

            _target = new DepositCommandHandler(_walletRepositoryMock.Object, _currencyRepositoryMock.Object, _authenticationServiceMock.Object);
        }

        [TestCase]
        public void WhenUserIsNotLoggedIn_ThenDepositCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock
                .Setup(x => x.CheckLoggedInUser())
                .Throws(new UserNotLoggedInException(ExceptionMessage.UserNotLoggedInExceptionMessage));

            _target.Invoking(async x => await x.Handle(_depositCommand, CancellationToken.None)).Should().Throw<UserNotLoggedInException>();
        }

        [TestCase]
        public void WhenCurrencyIsNull_ThenDepositCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock
                .Setup(x => x.CheckLoggedInUser());

            _currencyRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Currency, bool>>>(), null, false));

            _target.Invoking(async x => await x.Handle(_depositCommand, CancellationToken.None)).Should().Throw<CurrencyNotFoundException>();
        }

        [TestCase]
        public void WhenWalletIsNull_ThenDepositCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock
                .Setup(x => x.CheckLoggedInUser());

            _currencyRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Currency, bool>>>(), null, false))
                .Returns(_currencies);

            _walletRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Wallet, bool>>>(), null, false));

            _target.Invoking(async x => await x.Handle(_depositCommand, CancellationToken.None)).Should().Throw<WalletNotFoundException>();
        }

        [TestCase]
        public void WhenMoneyIsLessThanOrEqualToZero_ThenDepositCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock
                .Setup(x => x.CheckLoggedInUser());

            _currencyRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Currency, bool>>>(), null, false))
                .Returns(_currencies);

            _walletRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Wallet, bool>>>(), null, false))
                .Returns(_wallets);

            _target.Invoking(async x => await x.Handle(_depositCommandWithZeroMoney, CancellationToken.None)).Should().Throw<InvalidOperationException>();
        }

        [TestCase]
        public void WhenMoneyIsGreaterThanZero_ThenDepositCommandHandlerShouldNotThrowAnException()
        {
            _authenticationServiceMock
                .Setup(x => x.CheckLoggedInUser());

            _currencyRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Currency, bool>>>(), null, false))
                .Returns(_currencies);

            _walletRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Wallet, bool>>>(), null, false))
                .Returns(_wallets);

            _walletRepositoryMock.Setup(x => x.Update(It.IsAny<Wallet>()));

            _target.Invoking(async x => await x.Handle(_depositCommand, CancellationToken.None)).Should().NotThrow();
        }
    }
}