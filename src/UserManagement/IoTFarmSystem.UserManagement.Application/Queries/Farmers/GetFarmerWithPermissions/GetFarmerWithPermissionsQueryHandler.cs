using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;


namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerWithPermissions
{

    public class GetFarmerWithPermissionsQueryHandler : IRequestHandler<GetFarmerWithPermissionsQuery, Farmer?>
    {
        private readonly IFarmerRepository _farmerRepository;

        public GetFarmerWithPermissionsQueryHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<Farmer?> Handle(GetFarmerWithPermissionsQuery request, CancellationToken cancellationToken) =>
            await _farmerRepository.GetWithPermissionsAsync(request.FarmerId, cancellationToken);
    }
}
