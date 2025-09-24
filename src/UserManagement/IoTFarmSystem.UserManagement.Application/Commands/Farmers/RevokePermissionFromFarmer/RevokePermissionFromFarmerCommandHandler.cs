using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokePermissionFromFarmer
{
    public class RevokePermissionFromFarmerCommandHandler
        : IRequestHandler<RevokePermissionFromFarmerCommand, Unit>
    {
        private readonly IFarmerRepository _farmerRepository;

        public RevokePermissionFromFarmerCommandHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<Unit> Handle(RevokePermissionFromFarmerCommand request, CancellationToken cancellationToken)
        {
            // Step 1. Load Farmer aggregate with permissions
            var farmer = await _farmerRepository.GetWithPermissionsAsync(request.FarmerId, cancellationToken)
                            ?? throw new KeyNotFoundException($"Farmer '{request.FarmerId}' not found");

            // Step 2. Ensure farmer has the permission
            var existingPermission = farmer.Permissions
                                            .FirstOrDefault(p => p.PermissionName == request.PermissionName);

            if (existingPermission == null)
                throw new KeyNotFoundException(
                    $"Permission '{request.PermissionName}' not found for farmer '{request.FarmerId}'");

            // Step 3. Revoke it
            farmer.RevokePermission(request.PermissionName);

            // Step 4. Persist changes
            await _farmerRepository.UpdateAsync(farmer, cancellationToken);

            return Unit.Value;
        }
    }
}

