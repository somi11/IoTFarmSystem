using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.GrantPermissionToFarmer;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.Contracts.Services;
using MediatR;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers
{
    public class GrantPermissionToFarmerCommandHandler
        : IRequestHandler<GrantPermissionToFarmerCommand, Result<Unit>>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IPermissionLookupService _permissionLookup;
        private readonly IUnitOfWork _unitOfWork;

        public GrantPermissionToFarmerCommandHandler(
            IFarmerRepository farmerRepository,
            ITenantRepository tenantRepository,
            IPermissionLookupService permissionLookup,
            IUnitOfWork unitOfWork)
        {
            _farmerRepository = farmerRepository;
            _tenantRepository = tenantRepository;
            _permissionLookup = permissionLookup;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(GrantPermissionToFarmerCommand request, CancellationToken cancellationToken)
        {
            // Step 1. Load farmer with permissions
            var farmer = await _farmerRepository.GetWithPermissionsAsync(request.FarmerId, cancellationToken);
            if (farmer == null)
                return Result<Unit>.Fail($"Farmer '{request.FarmerId}' not found");

            // Step 2. Load tenant to ensure farmer belongs to a valid tenant
            var tenant = await _tenantRepository.GetByIdAsync(farmer.TenantId, cancellationToken);
            if (tenant == null)
                return Result<Unit>.Fail($"Tenant '{farmer.TenantId}' not found for farmer '{request.FarmerId}'");

            // Step 3. Look up permission by Id
            var permission = await _permissionLookup.GetByNameAsync(request.PermissionName, cancellationToken);
            if (permission == null)
                return Result<Unit>.Fail($"Permission '{request.PermissionName}' not found");

            // Step 4. Grant to farmer
            farmer.GrantPermission(permission);

            // Step 5. Persist
            await _farmerRepository.UpdateAsync(farmer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Ok(Unit.Value);
        }
    }
}
