using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokeRoleFromFarmer
{
    public class RevokeRoleFromFarmerCommandHandler : IRequestHandler<RevokeRoleFromFarmerCommand, Unit>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserService _userService;

        public RevokeRoleFromFarmerCommandHandler(
            IFarmerRepository farmerRepository,
            IRoleRepository roleRepository,
            IUserService userService)
        {
            _farmerRepository = farmerRepository;
            _roleRepository = roleRepository;
            _userService = userService;
        }

        public async Task<Unit> Handle(RevokeRoleFromFarmerCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.GetWithRolesAsync(request.FarmerId, cancellationToken)
                         ?? throw new KeyNotFoundException($"Farmer '{request.FarmerId}' not found");

            var role = await _roleRepository.GetByNameAsync(request.RoleName, cancellationToken)
                       ?? throw new KeyNotFoundException($"Role '{request.RoleName}' not found");

            // Remove the use of .Let, which does not exist. Instead, check if the role exists and call RevokeRole if so.
            var userRole = farmer.Roles.FirstOrDefault(r => r.RoleId == role.Id);
            if (userRole != null)
            {
                farmer.RevokeRole(role); // Domain removal
            }
            await _farmerRepository.RevokeRoleAsync(farmer, role, cancellationToken);

            // Sync with Identity
            await _userService.RemoveRoleAsync(farmer.IdentityUserId, role.Name, cancellationToken);

            return Unit.Value;
        }
    }
}
