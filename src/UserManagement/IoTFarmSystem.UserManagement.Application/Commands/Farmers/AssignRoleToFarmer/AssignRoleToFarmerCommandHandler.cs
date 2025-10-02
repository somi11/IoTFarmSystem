using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.AssignRoleToFarmer;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.AssignRole
{
    public class AssignRoleToFarmerCommandHandler : IRequestHandler<AssignRoleToFarmerCommand , Result<Unit>>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public AssignRoleToFarmerCommandHandler(
            IFarmerRepository farmerRepository,
            IRoleRepository roleRepository,
            IUserService userService,
            IUnitOfWork unitOfWork
            )
        {
            _farmerRepository = farmerRepository;
            _roleRepository = roleRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(AssignRoleToFarmerCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.GetWithRolesAsync(request.FarmerId, cancellationToken);
                if(farmer == null)
                return Result<Unit>.Fail($"Farmer '{request.FarmerId}' not found");


            var role = await _roleRepository.GetByNameAsync(request.RoleName, cancellationToken);
                if(role == null)
                return Result<Unit>.Fail($"Role '{request.RoleName}' not found");
                     

            farmer.AssignRole(role);
            await _farmerRepository.AssignRoleAsync(farmer, role, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            // Sync with Identity
           // await _userService.AssignRoleAsync(farmer.IdentityUserId, role.Name, cancellationToken);


            return Result<Unit>.Ok(Unit.Value);
        }
    }
}
