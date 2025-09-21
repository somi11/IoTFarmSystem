using MediatR;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.UpdateFarmer
{
    public class UpdateFarmerCommandHandler : IRequestHandler<UpdateFarmerCommand , Unit>
    {
        private readonly IFarmerRepository _farmerRepository;

        public UpdateFarmerCommandHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<Unit> Handle(UpdateFarmerCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.GetByIdAsync(request.FarmerId, cancellationToken)
                         ?? throw new KeyNotFoundException($"Farmer '{request.FarmerId}' not found");

            if (!string.IsNullOrWhiteSpace(request.Name))
                farmer.UpdateName(request.Name);

            // Optionally update email in domain or via IUserService (not shown here)

            await _farmerRepository.UpdateAsync(farmer, cancellationToken);
            return Unit.Value;
        }
    }
}
