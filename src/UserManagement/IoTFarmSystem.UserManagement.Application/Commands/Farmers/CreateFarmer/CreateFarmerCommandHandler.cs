using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.SharedKernel.Security;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.CreateFarmer;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.Contracts.Services;
using MediatR;
using Microsoft.Extensions.Logging;

public class CreateFarmerCommandHandler : IRequestHandler<CreateFarmerCommand, Result<Guid>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionLookupService _permissionLookup;
    private readonly ILogger<CreateFarmerCommandHandler> _logger;

    public CreateFarmerCommandHandler(
        ITenantRepository tenantRepository,
        IUserService userService,
        IUnitOfWork unitOfWork,
        IRoleRepository roleRepository,
        IPermissionLookupService permissionLookup,
        ILogger<CreateFarmerCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _userService = userService;
        _unitOfWork = unitOfWork;
        _roleRepository = roleRepository;
        _permissionLookup = permissionLookup;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateFarmerCommand request, CancellationToken cancellationToken)
    {
        if (request.Roles?.Contains(SystemRoles.SYSTEM_ADMIN) == true)
            return Result<Guid>.Fail("SystemAdmin cannot be created as a Farmer.");

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
            if (tenant is null)
                return Result<Guid>.Fail("Tenant not found");

            _logger.LogInformation("Tenant loaded: {TenantId}", tenant.Id);

            var identityUserId = await _userService.CreateUserAsync(request.Email, request.Password, cancellationToken);

            if (request.Roles?.Contains(SystemRoles.TENANT_OWNER) == true && tenant.HasOwner())
                return Result<Guid>.Fail("Each tenant can only have one owner.");

            var farmer = tenant.RegisterFarmer(Guid.NewGuid(), identityUserId, request.Email, request.Name);
            _logger.LogInformation("Farmer registered: {FarmerId}", farmer.Id);

            ////for in memorydb
            if (_unitOfWork.IsInMemoryDatabase())
            {
                _unitOfWork.DbContext.Set<Farmer>().Add(farmer);
            }
            // Assign roles
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

            // Assign permissions
            if (request.Permissions != null)
            {
                var explicitPermissions = await _permissionLookup.GetByNamesAsync(request.Permissions.Distinct(), cancellationToken);
                foreach (var perm in explicitPermissions)
                    farmer.GrantPermission(perm);
            }

            _logger.LogInformation("Saving changes for Tenant {TenantId} with {FarmerCount} farmers.", tenant.Id, tenant.Farmers.Count);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Transaction committed successfully for Farmer {FarmerId}", farmer.Id);

            return Result<Guid>.Ok(farmer.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating farmer.");
            await transaction.RollbackAsync(cancellationToken);
            return Result<Guid>.Fail($"Unexpected error: {ex.Message}");
        }
    }
}
