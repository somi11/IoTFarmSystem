using IoTFarmSystem.UserManagement.Application.Commands.Farmers.GrantPermissionToFarmer;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.Contracts.Services;
using MediatR;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers
{
    public class GrantPermissionToFarmerCommandHandler
        : IRequestHandler<GrantPermissionToFarmerCommand, Unit>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IPermissionLookupService _permissionLookup;

        public GrantPermissionToFarmerCommandHandler(
            IFarmerRepository farmerRepository,
            IPermissionLookupService permissionLookup)
        {
            _farmerRepository = farmerRepository;
            _permissionLookup = permissionLookup;
        }

        public async Task<Unit> Handle(GrantPermissionToFarmerCommand request, CancellationToken cancellationToken)
        {
            // 1. Load farmer
            var farmer = await _farmerRepository.GetWithRolesAsync(request.FarmerId, cancellationToken)
                         ?? throw new KeyNotFoundException($"Farmer '{request.FarmerId}' not found");

            // 2. Look up permission in DB (not create new)
            var permission = await _permissionLookup.GetByNameAsync(request.PermissionName, cancellationToken);
            if (permission is null)
                throw new KeyNotFoundException($"Permission '{request.PermissionName}' not found in system.");

            // 3. Grant to farmer
            farmer.GrantPermission(permission);

            // 4. Persist
            await _farmerRepository.GrantPermissionAsync(farmer, permission, cancellationToken);

            return Unit.Value;
        }
    }
}
