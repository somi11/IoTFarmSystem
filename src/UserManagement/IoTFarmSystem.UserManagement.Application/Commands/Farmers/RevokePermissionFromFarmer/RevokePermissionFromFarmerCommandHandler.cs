using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokePermissionFromFarmer
{
    public class RevokePermissionFromFarmerCommandHandler
        : IRequestHandler<RevokePermissionFromFarmerCommand, Result<Unit>>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RevokePermissionFromFarmerCommandHandler(
            IFarmerRepository farmerRepository,
            ITenantRepository tenantRepository,
            IUnitOfWork unitOfWork)
        {
            _farmerRepository = farmerRepository;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(RevokePermissionFromFarmerCommand request, CancellationToken cancellationToken)
        {
            // Step 1. Load farmer with permissions
            var farmer = await _farmerRepository.GetWithPermissionsAsync(request.FarmerId, cancellationToken);
            if (farmer == null)
                return Result<Unit>.Fail($"Farmer '{request.FarmerId}' not found");

            // Step 2. Load tenant to ensure farmer belongs to a valid tenant
            var tenant = await _tenantRepository.GetByIdAsync(farmer.TenantId, cancellationToken);
            if (tenant == null)
                return Result<Unit>.Fail($"Tenant '{farmer.TenantId}' not found for farmer '{request.FarmerId}'");

            // Step 3. Ensure farmer has the permission
            var existingPermission = farmer.ExplicitPermissions
                .FirstOrDefault(p => p.PermissionName == request.PermissionName);

            if (existingPermission == null)
                return Result<Unit>.Fail(
                    $"Permission '{request.PermissionName}' not found for farmer '{request.FarmerId}'");

            // Step 4. Revoke it
            farmer.RevokePermission(existingPermission.PermissionId);

            // Step 5. Persist
            await _farmerRepository.UpdateAsync(farmer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Ok(Unit.Value);
        }
    }
}
