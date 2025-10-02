using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;


namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerWithPermissions
{

    public class GetFarmerWithPermissionsQueryHandler : IRequestHandler<GetFarmerWithPermissionsQuery, FarmerDto?>
    {
        private readonly IFarmerRepository _farmerRepository;

        public GetFarmerWithPermissionsQueryHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<FarmerDto?> Handle(GetFarmerWithPermissionsQuery request, CancellationToken cancellationToken) =>
            await _farmerRepository.GetWithPermissionsQueryAsync(request.FarmerId, cancellationToken);
    }
}
