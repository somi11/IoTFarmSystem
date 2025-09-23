using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.UpdateFarmer
{
    public class UpdateFarmerCommandHandler : IRequestHandler<UpdateFarmerCommand , Unit>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IUserService _userService;
        public UpdateFarmerCommandHandler(
                   IFarmerRepository farmerRepository,
                   IUserService userService)
        {
            _farmerRepository = farmerRepository;
            _userService = userService;
        }

        public async Task<Unit> Handle(UpdateFarmerCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.GetEntityByIdAsync(request.FarmerId, cancellationToken)
                             ?? throw new KeyNotFoundException($"Farmer '{request.FarmerId}' not found");

            // Update Name
            if (!string.IsNullOrWhiteSpace(request.Name))
                farmer.UpdateName(request.Name);

            // Update Email (domain + Identity system)
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != farmer.Email)
            {
                // Update in Identity provider
                await _userService.UpdateEmailAsync(farmer.IdentityUserId, request.Email, cancellationToken);

                // Update in domain
                farmer.UpdateEmail(request.Email);
            }

            await _farmerRepository.UpdateAsync(farmer, cancellationToken);
            return Unit.Value;
        }

    }
}
