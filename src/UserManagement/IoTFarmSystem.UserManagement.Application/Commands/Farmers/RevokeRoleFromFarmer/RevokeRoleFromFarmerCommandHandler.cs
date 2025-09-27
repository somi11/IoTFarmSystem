using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.Contracts.Services;
using MediatR;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokeRoleFromFarmer
{
    public class RevokeRoleFromFarmerCommandHandler
        : IRequestHandler<RevokeRoleFromFarmerCommand, Result<Unit>>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public RevokeRoleFromFarmerCommandHandler(
            IFarmerRepository farmerRepository,
            IRoleRepository roleRepository,
            ITenantRepository tenantRepository,
            IUnitOfWork unitOfWork,
            IUserService userService)
        {
            _farmerRepository = farmerRepository;
            _roleRepository = roleRepository;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<Result<Unit>> Handle(RevokeRoleFromFarmerCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Load farmer with roles & permissions
            var farmer = await _farmerRepository.GetWithRolesAndPermissionsAsync(request.FarmerId, cancellationToken);
            if (farmer == null)
                return Result<Unit>.Fail($"Farmer '{request.FarmerId}' not found");

            // Step 2: Load tenant
            var tenant = await _tenantRepository.GetByIdAsync(farmer.TenantId, cancellationToken);
            if (tenant == null)
                return Result<Unit>.Fail($"Tenant '{farmer.TenantId}' not found");

            // Step 3: Load role
            var role = await _roleRepository.GetByNameAsync(request.RoleName, cancellationToken);
            if (role == null)
                return Result<Unit>.Fail($"Role '{request.RoleName}' not found");

            // Step 4: Revoke role + permissions
            farmer.RevokeRoleWithPermissions(role);

            // Step 5: Persist changes
            await _farmerRepository.UpdateAsync(farmer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Optional: sync with Identity
            //await _userService.RemoveRoleAsync(farmer.IdentityUserId, role.Name, cancellationToken);

            return Result<Unit>.Ok(Unit.Value);
        }
    }
}

