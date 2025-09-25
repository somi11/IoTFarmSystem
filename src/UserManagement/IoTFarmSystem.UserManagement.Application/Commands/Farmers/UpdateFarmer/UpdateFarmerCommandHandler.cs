using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.UpdateFarmer
{
    public class UpdateFarmerCommandHandler : IRequestHandler<UpdateFarmerCommand, Result<Unit>>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IUserService _userService; 
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFarmerCommandHandler(
            IFarmerRepository farmerRepository,
           IUnitOfWork unitOfWork,
        IUserService userService)
        {
            _farmerRepository = farmerRepository;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<Result<Unit>> Handle(UpdateFarmerCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Load Farmer
            var farmer = await _farmerRepository.GetEntityByIdAsync(request.FarmerId, cancellationToken);
            if (farmer == null)
                return Result<Unit>.Fail($"Farmer '{request.FarmerId}' not found");

            // Step 2: Update Name
            if (!string.IsNullOrWhiteSpace(request.Name))
                farmer.UpdateName(request.Name);

            // Step 3: Update Email (domain + Identity)
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != farmer.Email)
            {
                try
                {
                    await _userService.UpdateEmailAsync(farmer.IdentityUserId, request.Email, cancellationToken);
                    farmer.UpdateEmail(request.Email);
                }
                catch (Exception ex)
                {
                    return Result<Unit>.Fail($"Failed to update email: {ex.Message}");
                }

                farmer.UpdateEmail(request.Email);
            }

            // Step 4: Persist domain changes
            await _farmerRepository.UpdateAsync(farmer, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            return Result<Unit>.Ok(Unit.Value);
        }
    }
}
