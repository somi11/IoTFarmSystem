using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.DTOs;
using IoTFarmSystem.UserManagement.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace IoTFarmSystem.UserManagement.Infrastructure.Persistance.Repositories
{
    public class FarmerRepository : IFarmerRepository
    {
        private readonly UserManagementDbContext _dbContext;

        public FarmerRepository(UserManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Farmer?> GetEntityByIdAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .FirstOrDefaultAsync(f => f.Id == farmerId, cancellationToken);
        }
        public async Task<FarmerDto?> GetByIdAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            var farmer = await _dbContext.Farmers
                .Where(f => f.Id == farmerId)
                .Select(f => new FarmerDto
                {
                    Id = f.Id,
                    TenantId = f.TenantId,
                    Email = f.Email,
                    Name = f.Name,

                    Roles = EF.Property<List<UserRole>>(f, "_roles")
                        .Select(r => r.Role.Name)
                        .ToList(),

                    Permissions = EF.Property<List<UserRole>>(f, "_roles")
                        .SelectMany(r => EF.Property<List<RolePermission>>(r.Role, "_permissions")
                            .Select(p => p.Permission.Name))
                        .Union(EF.Property<List<UserPermission>>(f, "_permissions")
                            .Select(p => p.Permission.Name))
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return farmer;

        }

        public async Task<FarmerDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            //var farmer = await _dbContext.Farmers
            //    .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
            //        .ThenInclude(ur => ur.Role)
            //    .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
            //    .FirstOrDefaultAsync(f => f.Email == email, cancellationToken);

            //if (farmer == null)
            //    return null;

            //return new FarmerDto
            //{
            //    Id = farmer.Id,
            //    TenantId = farmer.TenantId,
            //    Email = farmer.Email,
            //    Name = farmer.Name,
            //    Roles = farmer.Roles.Select(r => r.Role.Name).ToList(),
            //    Permissions = farmer.ExplicitPermissions.Select(p => p.PermissionName).ToList()
            //};

            var farmer = await _dbContext.Farmers
                        .Where(f => f.Email == email)
                        .Select(f => new FarmerDto
                        {
                            Id = f.Id,
                            TenantId = f.TenantId,
                            Email = f.Email,
                            Name = f.Name,
                            Roles = EF.Property<List<UserRole>>(f, "_roles")
                                      .Select(r => r.Role.Name)
                                       .ToList(),
                            Permissions = EF.Property<List<UserRole>>(f, "_roles")
                            .SelectMany(r => EF.Property<List<RolePermission>>(r.Role, "_permissions")
                            .Select(p => p.Permission.Name))
                            .Union(EF.Property<List<UserPermission>>(f, "_permissions")
                            .Select(p => p.Permission.Name))
                        .ToList()
                        }).FirstOrDefaultAsync(cancellationToken);
            return farmer;
        }


        public async Task<IReadOnlyList<FarmerDto>> GetByTenantAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default)
        {
            var farmers = await _dbContext.Farmers
                      .Where(f => f.TenantId == tenantId)
                      .Select(f => new FarmerDto
                      {
                          Id = f.Id,
                          TenantId = f.TenantId,
                          Email = f.Email,
                          Name = f.Name,
                          Roles = EF.Property<List<UserRole>>(f, "_roles")
                                    .Select(r => r.Role.Name)
                                     .ToList(),
                          Permissions = EF.Property<List<UserRole>>(f, "_roles")
                          .SelectMany(r => EF.Property<List<RolePermission>>(r.Role, "_permissions")
                          .Select(p => p.Permission.Name))
                          .Union(EF.Property<List<UserPermission>>(f, "_permissions")
                          .Select(p => p.Permission.Name))
                      .ToList()
                      }).ToListAsync(cancellationToken);
            return farmers;
        }
        public async Task<Farmer?> GetWithRolesAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(f => f.Id == farmerId, cancellationToken);


        }
        public async Task<FarmerDto?> GetWithRolesQueryAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            var farmer = await _dbContext.Farmers
         .Where(f => f.Id == farmerId)
         .Select(
         f => new FarmerDto
         {
             Id = f.Id,
             TenantId = f.TenantId,
             Email = f.Email,
             Name = f.Name,
             Roles = EF.Property<List<UserRole>>(f, "_roles")
                             .Select(r => r.Role.Name)
                              .ToList()
         }).FirstOrDefaultAsync(cancellationToken);

            return farmer;


        }


        public async Task<Farmer?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                .FirstOrDefaultAsync(f => f.IdentityUserId == identityUserId, cancellationToken);
        }

        public async Task<Farmer?> GetWithPermissionsAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                    .ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(f => f.Id == farmerId, cancellationToken);
        }

        public async Task<FarmerDto?> GetWithPermissionsQueryAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            var farmers = await _dbContext.Farmers
          .Where(f => f.Id == farmerId)
          .Select(f => new FarmerDto
          {
              Id = f.Id,
              TenantId = f.TenantId,
              Email = f.Email,
              Name = f.Name,
              Roles = EF.Property<List<UserRole>>(f, "_roles")
                        .Select(r => r.Role.Name)
                         .ToList(),
              Permissions = EF.Property<List<UserRole>>(f, "_roles")
              .SelectMany(r => EF.Property<List<RolePermission>>(r.Role, "_permissions")
              .Select(p => p.Permission.Name))
              .Union(EF.Property<List<UserPermission>>(f, "_permissions")
              .Select(p => p.Permission.Name))
          .ToList()
          }).FirstOrDefaultAsync(cancellationToken);
            return farmers;
        }
        public async Task<IReadOnlyList<FarmerDto>> GetByRoleNameAsync(
            Guid tenantId,
            string roleName,
            CancellationToken cancellationToken = default)
        {
            var farmers = await _dbContext.Farmers
                .Where(f => f.TenantId == tenantId &&
                            EF.Property<List<UserRole>>(f, "_roles").Any(ur => ur.Role.Name == roleName))
                .Select(f => new FarmerDto
                {
                    Id = f.Id,
                    TenantId = f.TenantId,
                    Email = f.Email,
                    Name = f.Name,

                    Roles = EF.Property<List<UserRole>>(f, "_roles")
                                    .Select(r => r.Role.Name)
                                     .ToList(),
                    Permissions = EF.Property<List<UserRole>>(f, "_roles")
                          .SelectMany(r => EF.Property<List<RolePermission>>(r.Role, "_permissions")
                          .Select(p => p.Permission.Name))
                          .Union(EF.Property<List<UserPermission>>(f, "_permissions")
                          .Select(p => p.Permission.Name))
                      .ToList(),
                    CreatedAt = null,   // not mapped in your model, placeholder
                    LastLogin = null    // same
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return farmers;
        }


        public async Task<Farmer?> GetWithRolesAndPermissionsAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => EF.Property<List<RolePermission>>(r, "_permissions")) // include role permissions
                .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                    .ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(f => f.Id == farmerId, cancellationToken);
        }

        // ------------------ Mutating methods (no SaveChanges here) ------------------

        public Task AddAsync(Farmer farmer, CancellationToken cancellationToken = default)
        {
            return _dbContext.Farmers.AddAsync(farmer, cancellationToken).AsTask();
        }

        public Task UpdateAsync(Farmer farmer, CancellationToken cancellationToken = default)
        {
            _dbContext.Farmers.Update(farmer);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Farmer farmer, CancellationToken cancellationToken = default)
        {
            _dbContext.Farmers.Remove(farmer);
            return Task.CompletedTask;
        }

        public Task AssignRoleAsync(Farmer farmer, Role role, CancellationToken cancellationToken = default)
        {
            farmer.AssignRole(role);
            _dbContext.Farmers.Update(farmer);
            return Task.CompletedTask;
        }

        public Task RevokeRoleAsync(Farmer farmer, Role role, CancellationToken cancellationToken = default)
        {
            farmer.RevokeRole(role);
            _dbContext.Farmers.Update(farmer);
            return Task.CompletedTask;
        }

        public Task GrantPermissionAsync(Farmer farmer, Permission permission, CancellationToken cancellationToken = default)
        {
            farmer.GrantPermission(permission);
            _dbContext.Farmers.Update(farmer);
            return Task.CompletedTask;
        }

        public Task RevokePermissionAsync(Farmer farmer, Permission permission, CancellationToken cancellationToken = default)
        {
            farmer.RevokePermission(permission.Id);
            _dbContext.Farmers.Update(farmer);
            return Task.CompletedTask;
        }
    }
}
