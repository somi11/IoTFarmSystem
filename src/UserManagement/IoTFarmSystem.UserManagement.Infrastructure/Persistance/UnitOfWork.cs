using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace IoTFarmSystem.UserManagement.Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManagementDbContext _dbContext;
        public DbSet<Permission> Permissions => _dbContext.Permissions;
        public UnitOfWork(UserManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            // Check if provider supports transactions
            if (_dbContext.Database.IsInMemory())
            {
                // Return a dummy transaction for in-memory DB
                return new DummyUnitOfWorkTransaction(_dbContext);
            }

            // Real transaction for relational DBs
            var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            return new EfCoreUnitOfWorkTransaction(transaction, _dbContext);
        }

        public async ValueTask DisposeAsync() => await _dbContext.DisposeAsync();

        // -----------------------------
        // EF Core transaction wrapper
        // -----------------------------
        private class EfCoreUnitOfWorkTransaction : IUnitOfWorkTransaction
        {
            private readonly IDbContextTransaction _transaction;
            private readonly UserManagementDbContext _dbContext;

            public EfCoreUnitOfWorkTransaction(IDbContextTransaction transaction, UserManagementDbContext dbContext)
            {
                _transaction = transaction;
                _dbContext = dbContext;
            }

            public async Task CommitAsync(CancellationToken cancellationToken = default)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }

            public async Task RollbackAsync(CancellationToken cancellationToken = default)
            {
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

            public DummyUnitOfWorkTransaction(UserManagementDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task CommitAsync(CancellationToken cancellationToken = default)
            {
                // Save changes but no real transaction
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            public Task RollbackAsync(CancellationToken cancellationToken = default)
            {
                // No-op for in-memory
                return Task.CompletedTask;
            }

            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }
}
