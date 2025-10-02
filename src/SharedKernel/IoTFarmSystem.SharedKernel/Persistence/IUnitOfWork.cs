using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IoTFarmSystem.SharedKernel.Persistence
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        bool IsInMemoryDatabase();
        DbContext DbContext { get; }
    }

    public interface IUnitOfWorkTransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
