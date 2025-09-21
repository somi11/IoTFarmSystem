using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Contracts.Persistance
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }

    public interface IUnitOfWorkTransaction : IAsyncDisposable
    {
        /// <summary>
        /// Commit all changes within the transaction.
        /// </summary>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rollback all changes within the transaction.
        /// </summary>
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
