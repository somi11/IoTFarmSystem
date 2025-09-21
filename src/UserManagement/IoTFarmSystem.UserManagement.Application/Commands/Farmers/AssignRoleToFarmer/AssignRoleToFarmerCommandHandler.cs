using IoTFarmSystem.UserManagement.Application.Commands.Farmers.AssignRoleToFarmer;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.AssignRole
{
    public class AssignRoleToFarmerCommandHandler : IRequestHandler<AssignRoleToFarmerCommand , Unit>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserService _userService;

        public AssignRoleToFarmerCommandHandler(
            IFarmerRepository farmerRepository,
            IRoleRepository roleRepository,
            IUserService userService)
        {
            _farmerRepository = farmerRepository;
            _roleRepository = roleRepository;
            _userService = userService;
        }

        public async Task<Unit> Handle(AssignRoleToFarmerCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.GetWithRolesAsync(request.FarmerId, cancellationToken)
                         ?? throw new KeyNotFoundException($"Farmer '{request.FarmerId}' not found");

            var role = await _roleRepository.GetByNameAsync(request.RoleName, cancellationToken)
                       ?? throw new KeyNotFoundException($"Role '{request.RoleName}' not found");

            farmer.AssignRole(role);
            await _farmerRepository.AssignRoleAsync(farmer, role, cancellationToken);

            // Sync with Identity
            await _userService.AssignRoleAsync(farmer.IdentityUserId, role.Name, cancellationToken);

            return Unit.Value;
        }
    }
}
