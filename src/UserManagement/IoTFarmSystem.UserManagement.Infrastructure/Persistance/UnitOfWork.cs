using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace IoTFarmSystem.UserManagement.Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManagementDbContext _dbContext;
        private readonly ILogger<UnitOfWork> _logger;


        public UnitOfWork(UserManagementDbContext dbContext, ILogger<UnitOfWork> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public bool IsInMemoryDatabase() => _dbContext.Database.IsInMemory();
        /// <summary>
        /// Begin transaction (dummy for in-memory, real for relational DB)
        /// </summary>
        public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.Database.IsInMemory())
            {
                _logger.LogDebug("Using dummy transaction for in-memory database.");
                return new DummyUnitOfWorkTransaction(_dbContext, _logger);
            }

            var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("EF Core transaction started.");
            return new EfCoreUnitOfWorkTransaction(transaction, _dbContext, _logger);
        }

        /// <summary>
        /// Save all tracked changes
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency exception while saving changes.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception while saving changes.");
                throw;
            }
        }

        public async ValueTask DisposeAsync() => await _dbContext.DisposeAsync();

  
        public DbContext DbContext => _dbContext; // expose for manual attach in handler if needed

        // -----------------------------
        // EF Core transaction wrapper
        // -----------------------------
        private class EfCoreUnitOfWorkTransaction : IUnitOfWorkTransaction
        {
            private readonly IDbContextTransaction _transaction;
            private readonly UserManagementDbContext _dbContext;
            private readonly ILogger _logger;

            public EfCoreUnitOfWorkTransaction(IDbContextTransaction transaction, UserManagementDbContext dbContext, ILogger logger)
            {
                _transaction = transaction;
                _dbContext = dbContext;
                _logger = logger;
            }

            public async Task CommitAsync(CancellationToken cancellationToken = default)
            {
                _logger.LogDebug("Committing EF Core transaction.");
                await _dbContext.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }

            public async Task RollbackAsync(CancellationToken cancellationToken = default)
            {
                _logger.LogDebug("Rolling back EF Core transaction.");
                await _transaction.RollbackAsync(cancellationToken);
            }

            public async ValueTask DisposeAsync() => await _transaction.DisposeAsync();
        }

        // -----------------------------
        // Dummy transaction for InMemory
        // -----------------------------
        private class DummyUnitOfWorkTransaction : IUnitOfWorkTransaction
        {
            private readonly UserManagementDbContext _dbContext;
            private readonly ILogger _logger;

            public DummyUnitOfWorkTransaction(UserManagementDbContext dbContext, ILogger logger)
            {
                _dbContext = dbContext;
                _logger = logger;
            }

            public async Task CommitAsync(CancellationToken cancellationToken = default)
            {
                _logger.LogDebug("Committing dummy transaction (in-memory DB).");
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            public Task RollbackAsync(CancellationToken cancellationToken = default)
            {
                _logger.LogDebug("Rollback skipped for in-memory DB.");
                return Task.CompletedTask;
            }

            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }
}
