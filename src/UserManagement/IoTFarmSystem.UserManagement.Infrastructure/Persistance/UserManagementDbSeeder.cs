using IoTFarmSystem.SharedKernel.Security;
using IoTFarmSystem.UserManagement.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace IoTFarmSystem.UserManagement.Infrastructure.Persistance
{
    public static class UserManagementDbSeeder
    {
        public static async Task SeedAsync(UserManagementDbContext dbContext)
        {
            if (await dbContext.Roles.AnyAsync())
                return;

            // 1. Collect all distinct permissions
            var allPermissionNames = RolePermissionsMap.Map
                .SelectMany(kvp => kvp.Value)
                .Distinct()
                .ToList();

            // 2. Create Permission entities with explicit IDs
            var permissions = allPermissionNames
                .Select(p => new Permission(Guid.NewGuid(), p))
                .ToList();

            await dbContext.Permissions.AddRangeAsync(permissions);

            // 3. Create roles and attach permissions
            foreach (var kvp in RolePermissionsMap.Map)
            {
                var role = new Role(Guid.NewGuid(), kvp.Key);

                foreach (var permName in kvp.Value)
                {
                    var perm = permissions.First(p => p.Name == permName);
                    role.AddPermission(perm);
                }

                dbContext.Roles.Add(role);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
