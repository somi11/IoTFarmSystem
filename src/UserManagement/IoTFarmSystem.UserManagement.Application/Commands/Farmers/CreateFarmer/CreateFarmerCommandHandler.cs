using IoTFarmSystem.UserManagement.Application.Contracts;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Domain.Entites;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.CreateFarmer
{
    public class CreateFarmerCommandHandler : IRequestHandler<CreateFarmerCommand, Guid>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;

        public CreateFarmerCommandHandler(
            IFarmerRepository farmerRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepository)
        {
            _farmerRepository = farmerRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
        }

        public async Task<Guid> Handle(CreateFarmerCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Create Identity user
                var identityUserId = await _userService.CreateUserAsync(request.Email, request.Password, cancellationToken);

                // 2. Create domain Farmer
                var farmer = new Farmer(identityUserId, request.Email, request.TenantId);

                // 3. Assign roles if provided
                if (request.Roles != null)
                {
                    foreach (var roleName in request.Roles)
                    {
                        var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken)
                                   ?? throw new KeyNotFoundException($"Role '{roleName}' not found");
                        farmer.AssignRole(role);
                        await _farmerRepository.AssignRoleAsync(farmer, role, cancellationToken);
                        await _userService.AssignRoleAsync(identityUserId, roleName, cancellationToken);
                    }
                }

                // 4. Grant permissions if provided
                if (request.Permissions != null)
                {
                    foreach (var permissionName in request.Permissions)
                    {
                        var permission = new Permission(permissionName); // create domain permission
                        farmer.GrantPermission(permission);
                        await _farmerRepository.GrantPermissionAsync(farmer, permission, cancellationToken);
                    }
                }

                // 5. Persist Farmer
                await _farmerRepository.AddAsync(farmer, cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return farmer.Id;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
