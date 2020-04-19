using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TradingEngine.Domain.AccountHistoryDomain;
using TradingEngine.Domain.CurrencyDomain;
using TradingEngine.Domain.UserDomain;
using TradingEngine.Domain.WalletDomain;
using TradingEngine.Extensions;
using TradingEngine.Infrastructure.EntityTypeConfigurations;

namespace TradingEngine.Infrastructure.Context
{
    public class TradingEngineContext : DbContext
    {
        private readonly IMediator _mediator;

        public TradingEngineContext(DbContextOptions<TradingEngineContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public TradingEngineContext(DbContextOptions<TradingEngineContext> dbContextOptions, IMediator mediator) : base(dbContextOptions)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WalletEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DispatchDomainEventsAsync(this);

            await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AccountHistory> AccountHistories { get; set; }
        public DbSet<Currency> Currencies { get; set; }
    }
}