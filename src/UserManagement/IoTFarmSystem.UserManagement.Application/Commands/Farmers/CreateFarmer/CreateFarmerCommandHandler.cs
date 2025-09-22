using IoTFarmSystem.SharedKernel.Security;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Domain.Entites;
using MediatR;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.CreateFarmer
{
    public class CreateFarmerCommandHandler : IRequestHandler<CreateFarmerCommand, Guid>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;
        private readonly ITenantRepository _tenantRepository;

        public CreateFarmerCommandHandler(
            IFarmerRepository farmerRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepository,
            ITenantRepository tenantRepository)
        {
            _farmerRepository = farmerRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<Guid> Handle(CreateFarmerCommand request, CancellationToken cancellationToken)
        {
            if (request.Roles?.Contains(SystemRoles.SYSTEM_ADMIN) == true)
                throw new InvalidOperationException("SystemAdmin cannot be created as a Farmer.");

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Create Identity user
                var identityUserId = await _userService.CreateUserAsync(request.Email, request.Password, cancellationToken);

                // 2. Find or create Tenant
                Tenant tenant;
                if (request.TenantId.HasValue)
                {
                    tenant = await _tenantRepository.GetByIdAsync(request.TenantId.Value, cancellationToken)
                             ?? throw new KeyNotFoundException($"Tenant '{request.TenantId}' not found");
                }
                else
                {
                    tenant = new Tenant(request.Name + " Farm");
                    await _tenantRepository.AddAsync(tenant, cancellationToken);
                }

                // 3. Register Farmer under Tenant
                var farmer = tenant.RegisterFarmer(identityUserId, request.Email);

                // 4. Assign roles and permissions in-memory only (no DB updates yet)
                if (request.Roles != null)
                {
                    foreach (var roleName in request.Roles.Distinct())
                    {
                        if (!RolePermissionsMap.Map.ContainsKey(roleName))
                            continue; // skip invalid roles

                        var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken)
                                   ?? throw new KeyNotFoundException($"Role '{roleName}' not found in DB");

                        if (!farmer.Roles.Any(r => r.RoleId == role.Id))
                            farmer.AssignRole(role); // in-memory

                        foreach (var permName in RolePermissionsMap.Map[roleName].Distinct())
                        {
                            if (!farmer.Permissions.Any(p => p.PermissionName == permName))
                                farmer.GrantPermission(new Permission(permName));
                        }
                    }
                }

                // 5. Assign explicit permissions in-memory
                if (request.Permissions != null)
                {
                    var allKernelPermissions = RolePermissionsMap.Map.Values.SelectMany(x => x).ToHashSet();
                    foreach (var permName in request.Permissions.Distinct())
                    {
                        if (!allKernelPermissions.Contains(permName))
                            continue;

                        if (!farmer.Permissions.Any(p => p.PermissionName == permName))
                            farmer.GrantPermission(new Permission(permName));
                    }
                }

                // 6. Persist farmer with all roles & permissions in a single AddAsync call
                await _farmerRepository.AddAsync(farmer, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return farmer.Id;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
