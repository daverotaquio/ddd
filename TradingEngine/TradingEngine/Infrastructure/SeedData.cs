using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using TradingEngine.Infrastructure.Context;
using TradingEngine.Models.AccountHistoryEntity;
using TradingEngine.Models.CurrencyEntity;
using TradingEngine.Models.UserEntity;
using TradingEngine.Models.ValueObjects;
using TradingEngine.Models.WalletEntity;

namespace TradingEngine.Infrastructure
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TradingEngineContext(
                serviceProvider.GetRequiredService<DbContextOptions<TradingEngineContext>>()))
            {
                var user1 = new User(new AccountNumber(Guid.NewGuid()), "Dave");
                var user2 = new User(new AccountNumber(Guid.NewGuid()), "Joriz");

                if (!context.Users.Any())
                {

                    context.Users.AddRange(
                        user1,
                        user2
                    );
                }

                if (!context.Currencies.Any())
                {
                    context.Currencies.AddRange(
                        new Currency("PHP", 1, "en-PH"),
                        new Currency("USD", 50, "en-US"),
                        new Currency("GBP", 45, "en-GB"),
                        new Currency("JPY", 0.45m, "en-JP"),
                        new Currency("CNY", 7, "zh-CN")
                        );
                }

                if (!context.Wallets.Any())
                {
                    context.Wallets.AddRange(
                        new Wallet(0, new WalletId(Guid.NewGuid()), new Money(200), user1.Id),
                        new Wallet(0, new WalletId(Guid.NewGuid()), new Money(100), user2.Id));
                }


                if (!context.AccountHistories.Any())
                {
                    context.AccountHistories.AddRange(
                        new AccountHistory(user1.Id, DateTime.Now, "New wallet created for Dave with an initial balance of ₱200.00."),
                        new AccountHistory(user2.Id, DateTime.Now, "New wallet created for Joriz with an initial balance of ₱100.00.")
                        );
                }

                context.SaveChanges();
            }
        }
    }
}