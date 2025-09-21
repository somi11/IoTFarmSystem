using IoTFarmSystem.SharedKernel.Security;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokePermissionFromFarmer;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Domain.Entites;
using MediatR;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokePermission
{
    public class RevokePermissionFromFarmerCommandHandler : IRequestHandler<RevokePermissionFromFarmerCommand , Unit>
    {
        private readonly IFarmerRepository _farmerRepository;

        public RevokePermissionFromFarmerCommandHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<Unit> Handle(RevokePermissionFromFarmerCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.GetWithRolesAsync(request.FarmerId, cancellationToken)
                         ?? throw new KeyNotFoundException($"Farmer '{request.FarmerId}' not found");

            // Validate permission name exists in SystemPermissions
            var validPermissionNames = typeof(SystemPermissions)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Select(f => f.GetValue(null)?.ToString())
                .ToList();

            if (!validPermissionNames.Contains(request.PermissionName))
                throw new KeyNotFoundException($"Permission '{request.PermissionName}' is not a valid system permission");

            // Find the UserPermission by permission name
            var userPermission = farmer.Permissions
                .FirstOrDefault(up => up.PermissionId == farmer.Permissions
                    .Select(p => p.PermissionId)
                    .FirstOrDefault(id => /* logic to map id to name, if available */ false)); // You need a mapping here

            if (userPermission == null)
                throw new KeyNotFoundException($"Permission '{request.PermissionName}' not found for farmer");

            farmer.RevokePermission(userPermission);
            await _farmerRepository.RevokePermissionAsync(farmer, userPermission.PermissionId, cancellationToken);

      

            return Unit.Value;
        }
    }
}
