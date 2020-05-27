using System.Threading;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TradingEngine.Entities.UserEntity;
using TradingEngine.Entities.UserEntity.Events;
using TradingEngine.Entities.WalletEntity;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Tests.EventHandlers
{
    public class DepositedMoneyEventHandlerTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<IAccountHistoryRepository> _accountHistoryRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        private DepositedMoney _depositedMoney;
        private Wallet _wallet;
        private User _user;

        private DepositedMoneyEventHandler _target;

        public DepositedMoneyEventHandlerTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _accountHistoryRepositoryMock = new Mock<IAccountHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();

            _depositedMoney = fixture.Build<DepositedMoney>()
                .Create();

            _wallet = fixture.Build<Wallet>()
                .Create();

            _user = fixture.Build<User>()
                .Create();

            _target = new DepositedMoneyEventHandler(_walletRepositoryMock.Object, _accountHistoryRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [TestCase]
        public void WhenWalletIsNull_ThenDepositedMoneyEventHandlerShouldThrowAnException()
        {
            _walletRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()));

            _userRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()));

            _target.Invoking(async x => await x.Handle(_depositedMoney, CancellationToken.None)).Should().Throw<WalletNotFoundException>();
        }

        [TestCase]
        public void WhenUserIsNull_ThenDepositedMoneyEventHandlerShouldThrowAnException()
        {
            _walletRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(_wallet);

            _userRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()));

            _target.Invoking(async x => await x.Handle(_depositedMoney, CancellationToken.None)).Should().Throw<UserNotFoundException>();
        }

        [TestCase]
        public void WhenWalletIsNotNullAndUserIsNotNull_ThenDepositedMoneyEventHandlerShouldNotThrowAnException()
        {
            _walletRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(_wallet);

            _userRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(_user);

            _target.Invoking(async x => await x.Handle(_depositedMoney, CancellationToken.None)).Should().NotThrow();
        }
    }
}