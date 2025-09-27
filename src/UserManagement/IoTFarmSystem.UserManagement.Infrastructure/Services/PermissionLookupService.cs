using IoTFarmSystem.UserManagement.Application.Contracts.Services;
using IoTFarmSystem.UserManagement.Domain.Entites;
using IoTFarmSystem.UserManagement.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace IoTFarmSystem.UserManagement.Infrastructure.Services
{
    public class PermissionLookupService : IPermissionLookupService
    {
        private readonly UserManagementDbContext _db;

        public PermissionLookupService(UserManagementDbContext db)
        {
            _db = db;
        }

        public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _db.Permissions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Permission>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken)
        {
            return await _db.Permissions
                .AsNoTracking()
                .Where(p => names.Contains(p.Name))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Permission>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken)
        {
            return await _db.RolePermissions
                .AsNoTracking()
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .ToListAsync(cancellationToken);
        }
    }
}
