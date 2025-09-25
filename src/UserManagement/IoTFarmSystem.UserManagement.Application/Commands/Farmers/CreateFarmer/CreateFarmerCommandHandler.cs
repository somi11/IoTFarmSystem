using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.SharedKernel.Security;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.CreateFarmer;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.Contracts.Services;
using IoTFarmSystem.UserManagement.Domain.Entites;
using MediatR;

public class CreateFarmerCommandHandler : IRequestHandler<CreateFarmerCommand, Result<Guid>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionLookupService _permissionLookup;

    public CreateFarmerCommandHandler(
        ITenantRepository tenantRepository,
        IUserService userService,
        IUnitOfWork unitOfWork,
        IRoleRepository roleRepository,
        IPermissionLookupService permissionLookup)
    {
        _tenantRepository = tenantRepository;
        _userService = userService;
        _unitOfWork = unitOfWork;
        _roleRepository = roleRepository;
        _permissionLookup = permissionLookup;
    }

    public async Task<Result<Guid>> Handle(CreateFarmerCommand request, CancellationToken cancellationToken)
    {
        if (request.Roles?.Contains(SystemRoles.SYSTEM_ADMIN) == true)
            return Result<Guid>.Fail("SystemAdmin cannot be created as a Farmer.");

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Identity user
            var identityUserId = await _userService.CreateUserAsync(request.Email, request.Password, cancellationToken);

            // 2. Tenant
            Tenant tenant;
            if (request.TenantId.HasValue)
            {
                tenant = await _tenantRepository.GetByIdAsync(request.TenantId.Value, cancellationToken);
                if (tenant is null)
                    return Result<Guid>.Fail($"Tenant '{request.TenantId}' not found");
            }
            else
            {
                var tenantName = request.TenantName ?? $"{request.Name} Farm";
                tenant = new Tenant(Guid.NewGuid(), tenantName);
                await _tenantRepository.AddAsync(tenant, cancellationToken);
            }

            // 3. Owner rule
            if (request.Roles?.Contains(SystemRoles.TENANT_OWNER) == true && tenant.HasOwner())
                return Result<Guid>.Fail("Each tenant can only have one owner.");

            // 4. Farmer aggregate root
            var farmer = tenant.RegisterFarmer(Guid.NewGuid(), identityUserId, request.Email, request.Name);

            // 5. Assign roles (only store role links, do NOT duplicate permissions here)
            if (request.Roles != null)
            {
                foreach (var roleName in request.Roles.Distinct())
                {
                    var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken);
                    if (role is null)
                        return Result<Guid>.Fail($"Role '{roleName}' not found in DB");

                    farmer.AssignRole(role);
                }
            }

            // 6. Assign explicit permissions (only these go into farmer._permissions)
            if (request.Permissions != null)
            {
                var explicitPermissions = await _permissionLookup.GetByNamesAsync(
                    request.Permissions.Distinct(),
                    cancellationToken);

                var missing = request.Permissions
                    .Except(explicitPermissions.Select(p => p.Name))
                    .ToList();

                if (missing.Any())
                    return Result<Guid>.Fail($"Invalid permissions: {string.Join(", ", missing)}");

                foreach (var perm in explicitPermissions)
                    farmer.GrantPermission(perm);
            }

            // 7. Commit unit of work
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
