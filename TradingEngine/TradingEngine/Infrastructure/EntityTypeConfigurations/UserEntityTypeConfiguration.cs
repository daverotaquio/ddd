using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingEngine.Domain.UserDomain;
using TradingEngine.Domain.ValueObjects;

namespace TradingEngine.Infrastructure.EntityTypeConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.AccountNumber).HasConversion(x => x.Value, x => new AccountNumber(x));
        }
    }
}