using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Entities.WalletEntity;

namespace TradingEngine.Infrastructure.EntityTypeConfigurations
{
    public class WalletEntityTypeConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.Property(x => x.WalletId).HasConversion(x => x.Value, x => new WalletId(x));
            builder.Property(x => x.Balance).HasConversion(x => x.Value, x => new Money(x));
        }
    }
}