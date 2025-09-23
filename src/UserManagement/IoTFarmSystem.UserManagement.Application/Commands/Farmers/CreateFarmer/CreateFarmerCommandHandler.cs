using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.SharedKernel.Security;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Domain.Entites;
using MediatR;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.CreateFarmer
{
    public class CreateFarmerCommandHandler : IRequestHandler<CreateFarmerCommand, Result<Guid>>
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

        public async Task<Result<Guid>> Handle(CreateFarmerCommand request, CancellationToken cancellationToken)
        {
            if (request.Roles?.Contains(SystemRoles.SYSTEM_ADMIN) == true)
                return Result<Guid>.Fail("SystemAdmin cannot be created as a Farmer.");

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Create Identity user
                var identityUserId = await _userService.CreateUserAsync(request.Email, request.Password, cancellationToken);

                // 2. Find or create Tenant
                Tenant tenant;
                if (request.TenantId.HasValue)
                {
                    tenant = await _tenantRepository.GetByIdAsync(request.TenantId.Value, cancellationToken);
                    if (tenant is null)
                        return Result<Guid>.Fail($"Tenant '{request.TenantId}' not found");
                }
                else
                {
                    var tenantName = request.TenantName ?? request.Name + " Farm";
                    tenant = new Tenant(Guid.NewGuid(), tenantName);
                    await _tenantRepository.AddAsync(tenant, cancellationToken);
                }

                // 3. Check Owner rule
                if (request.Roles?.Contains(SystemRoles.TENANT_OWNER) == true && tenant.HasOwner())
                    return Result<Guid>.Fail("Each tenant can only have one owner.");

                // 4. Register Farmer
                var farmer = tenant.RegisterFarmer(Guid.NewGuid(),identityUserId, request.Email, request.Name);

                // 5. Assign roles and permissions
                if (request.Roles != null)
                {
                    var distinctRoles = request.Roles.Distinct().ToList();

                    // 1. Validate roles
                    var unknownRoles = distinctRoles
                        .Where(r => !RolePermissionsMap.Map.ContainsKey(r))
                        .ToList();

                    if (unknownRoles.Any())
                        return Result<Guid>.Fail($"Unknown roles: {string.Join(", ", unknownRoles)}");

                    // 2. Fetch all role entities
                    var roles = new List<Role>();
                    foreach (var roleName in distinctRoles)
                    {
                        var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken);
                        if (role is null)
                            return Result<Guid>.Fail($"Role '{roleName}' not found in DB");

                        roles.Add(role);
                    }

                    // 3. Assign roles + permissions
                    foreach (var role in roles)
                    {
                        if (!farmer.Roles.Any(r => r.RoleId == role.Id))
                            farmer.AssignRole(role);

                        foreach (var permName in RolePermissionsMap.Map[role.Name].Distinct())
                        {
                            if (!farmer.Permissions.Any(p => p.PermissionName == permName))
                                farmer.GrantPermission(new Permission(permName));
                        }
                    }
                }

                // 6. Explicit permissions
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

                // 7. Save farmer
                await _farmerRepository.AddAsync(farmer, cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Result<Guid>.Ok(farmer.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<Guid>.Fail($"Unexpected error: {ex.Message}");
            }
        }


    }
}
