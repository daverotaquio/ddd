using System.Threading;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TradingEngine.Exceptions;
using TradingEngine.Infrastructure.Repositories.Interface;
using TradingEngine.Models.UserEntity;
using TradingEngine.Models.UserEntity.Events;
using TradingEngine.Models.WalletEntity;

namespace TradingEngine.Tests.EventHandlers
{
    public class WithdrewMoneyEventHandlerTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<IAccountHistoryRepository> _accountHistoryRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        private WithdrewMoney _withdrewMoney;
        private Wallet _wallet;
        private User _user;

        private WithdrewMoneyEventHandler _target;

        public WithdrewMoneyEventHandlerTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _accountHistoryRepositoryMock = new Mock<IAccountHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();

            _withdrewMoney = fixture.Build<WithdrewMoney>()
                .Create();

            _wallet = fixture.Build<Wallet>()
                .Create();

            _user = fixture.Build<User>()
                .Create();

            _target = new WithdrewMoneyEventHandler(_walletRepositoryMock.Object, _accountHistoryRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [TestCase]
        public void WhenWalletIsNull_ThenWithdrewMoneyEventHandlerShouldThrowAnException()
        {
            _walletRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()));

            _userRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()));

            _target.Invoking(async x => await x.Handle(_withdrewMoney, CancellationToken.None)).Should().Throw<WalletNotFoundException>();
        }

        [TestCase]
        public void WhenUserIsNull_ThenWithdrewMoneyEventHandlerShouldThrowAnException()
        {
            _walletRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(_wallet);

            _userRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()));

            _target.Invoking(async x => await x.Handle(_withdrewMoney, CancellationToken.None)).Should().Throw<UserNotFoundException>();
        }

        [TestCase]
        public void WhenWalletIsNotNullAndUserIsNotNull_ThenWithdrewMoneyEventHandlerShouldNotThrowAnException()
        {
            _walletRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(_wallet);

            _userRepositoryMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(_user);

            _target.Invoking(async x => await x.Handle(_withdrewMoney, CancellationToken.None)).Should().NotThrow();
        }
    }
}