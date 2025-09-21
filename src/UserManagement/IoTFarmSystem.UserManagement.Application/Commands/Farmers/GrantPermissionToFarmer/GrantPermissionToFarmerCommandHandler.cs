using IoTFarmSystem.UserManagement.Application.Commands.Farmers.GrantPermissionToFarmer;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Domain.Entites;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.GrantPermission
{
    public class GrantPermissionToFarmerCommandHandler : IRequestHandler<GrantPermissionToFarmerCommand , Unit>
    {
        private readonly IFarmerRepository _farmerRepository;

        public GrantPermissionToFarmerCommandHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<Unit> Handle(GrantPermissionToFarmerCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.GetWithRolesAsync(request.FarmerId, cancellationToken)
                         ?? throw new KeyNotFoundException($"Farmer '{request.FarmerId}' not found");

            var permission = new Permission(request.PermissionName);
            farmer.GrantPermission(permission);

            await _farmerRepository.GrantPermissionAsync(farmer, permission, cancellationToken);
            return Unit.Value;
        }
    }
}
