using System;
using System.Collections.Generic;
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
using TradingEngine.Infrastructure.Repositories.Interface;
using TradingEngine.Requests.Commands;

namespace TradingEngine.Tests.CommandHandlers
{
    public class WithdrawCommandHandlerTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;

        private WithdrawCommand _withdrawCommand;
        private WithdrawCommand _withdrawCommandWithHugeMoney;

        private IEnumerable<Currency> _currencies;
        private IEnumerable<Wallet> _wallets;
        private IEnumerable<Wallet> _walletsWithSmallBalance;

        private WithdrawCommandHandler _target;

        public WithdrawCommandHandlerTests()
        {
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _walletRepositoryMock = new Mock<IWalletRepository>();
        }

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();

            _withdrawCommand = fixture.Build<WithdrawCommand>()
                .FromFactory(() => new WithdrawCommand(1, CurrencySettings.DefaultCurrencyKey))
                .Create();

            _withdrawCommandWithHugeMoney = fixture.Build<WithdrawCommand>()
                .FromFactory(() => new WithdrawCommand(1000, CurrencySettings.DefaultCurrencyKey))
                .Create();

            _currencies = fixture.Build<Currency>()
                .FromFactory(() => new Currency("PHP", 1, "en-PH"))
                .CreateMany(5);

            _wallets = fixture.Build<Wallet>()
                .CreateMany(1);

            _walletsWithSmallBalance = fixture.Build<Wallet>()
                .FromFactory(() => new Wallet(1, new WalletId(Guid.NewGuid()), new Money(10), 1))
                .CreateMany(1);

            _target = new WithdrawCommandHandler(_walletRepositoryMock.Object, _currencyRepositoryMock.Object, _authenticationServiceMock.Object);
        }

        [TestCase]
        public void WhenUserIsNotLoggedIn_ThenWithdrawCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock
                .Setup(x => x.CheckLoggedInUser())
                .Throws(new UserNotLoggedInException(ExceptionMessage.UserNotLoggedInExceptionMessage));

            _target.Invoking(async x => await x.Handle(_withdrawCommand, CancellationToken.None)).Should().Throw<UserNotLoggedInException>();
        }

        [TestCase]
        public void WhenCurrencyIsNull_ThenWithdrawCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock
                .Setup(x => x.CheckLoggedInUser());

            _currencyRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Currency, bool>>>(), null, false));

            _target.Invoking(async x => await x.Handle(_withdrawCommand, CancellationToken.None)).Should().Throw<CurrencyNotFoundException>();
        }

        [TestCase]
        public void WhenWalletIsNull_ThenWithdrawCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock
                .Setup(x => x.CheckLoggedInUser());

            _currencyRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Currency, bool>>>(), null, false))
                .Returns(_currencies);

            _walletRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Wallet, bool>>>(), null, false));

            _target.Invoking(async x => await x.Handle(_withdrawCommand, CancellationToken.None)).Should().Throw<WalletNotFoundException>();
        }

        [TestCase]
        public void WhenMoneyIsGreaterThanTheWalletBalance_ThenWithdrawCommandHandlerShouldThrowAnException()
        {
            _authenticationServiceMock
                .Setup(x => x.CheckLoggedInUser());

            _currencyRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Currency, bool>>>(), null, false))
                .Returns(_currencies);

            _walletRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Wallet, bool>>>(), null, false))
                .Returns(_walletsWithSmallBalance);

            _target.Invoking(async x => await x.Handle(_withdrawCommandWithHugeMoney, CancellationToken.None)).Should().Throw<InsufficientFundsException>();
        }

        [TestCase]
        public void WhenMoneyIsGreaterThanZero_ThenWithdrawCommandHandlerShouldNotThrowAnException()
        {
            _authenticationServiceMock
                .Setup(x => x.CheckLoggedInUser());

            _currencyRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Currency, bool>>>(), null, false))
                .Returns(_currencies);

            _walletRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<Wallet, bool>>>(), null, false))
                .Returns(_wallets);

            _walletRepositoryMock.Setup(x => x.Update(It.IsAny<Wallet>()));

            _target.Invoking(async x => await x.Handle(_withdrawCommand, CancellationToken.None)).Should().NotThrow();
        }
    }
}