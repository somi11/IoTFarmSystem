using IoTFarmSystem.UserManagement.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Contracts.Services
{
    public interface IPermissionLookupService
    {
        Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Permission>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Permission>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken);
    }
}

