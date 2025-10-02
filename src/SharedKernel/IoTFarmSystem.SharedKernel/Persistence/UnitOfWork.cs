using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace IoTFarmSystem.SharedKernel.Persistence
{
    public class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private readonly ILogger<UnitOfWork<TDbContext>> _logger;

        public UnitOfWork(TDbContext dbContext, ILogger<UnitOfWork<TDbContext>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public DbContext DbContext => _dbContext;

        public bool IsInMemoryDatabase() => _dbContext.Database.IsInMemory();

        public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.Database.IsInMemory())
            {
                _logger.LogDebug("Using dummy transaction for in-memory database.");
                return new DummyTransaction(_dbContext, _logger);
            }

            var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            return new EfTransaction(transaction, _dbContext, _logger);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _dbContext.SaveChangesAsync(cancellationToken);

        public ValueTask DisposeAsync() => _dbContext.DisposeAsync();

        private class EfTransaction : IUnitOfWorkTransaction
        {
            private readonly IDbContextTransaction _transaction;
            private readonly TDbContext _dbContext;
            private readonly ILogger _logger;

            public EfTransaction(IDbContextTransaction transaction, TDbContext dbContext, ILogger logger)
            {
                _transaction = transaction;
                _dbContext = dbContext;
                _logger = logger;
            }

            public async Task CommitAsync(CancellationToken cancellationToken = default)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }

            public Task RollbackAsync(CancellationToken cancellationToken = default)
                => _transaction.RollbackAsync(cancellationToken);

            public ValueTask DisposeAsync() => _transaction.DisposeAsync();
        }

        private class DummyTransaction : IUnitOfWorkTransaction
        {
            private readonly TDbContext _dbContext;
            private readonly ILogger _logger;

            public DummyTransaction(TDbContext dbContext, ILogger logger)
            {
                _dbContext = dbContext;
                _logger = logger;
            }

            public Task CommitAsync(CancellationToken cancellationToken = default)
                => _dbContext.SaveChangesAsync(cancellationToken);

            public Task RollbackAsync(CancellationToken cancellationToken = default)
            {
                _logger.LogDebug("Rollback skipped for in-memory DB.");
                return Task.CompletedTask;
            }

            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }
}
