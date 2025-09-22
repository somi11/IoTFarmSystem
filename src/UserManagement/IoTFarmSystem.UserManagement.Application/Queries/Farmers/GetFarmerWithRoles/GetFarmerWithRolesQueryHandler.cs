using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerWithRoles
{
    public class GetFarmerWithRolesQueryHandler : IRequestHandler<GetFarmerWithRolesQuery, Farmer?>
    {
        private readonly IFarmerRepository _farmerRepository;

        public GetFarmerWithRolesQueryHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<Farmer?> Handle(GetFarmerWithRolesQuery request, CancellationToken cancellationToken) =>
            await _farmerRepository.GetWithRolesAsync(request.FarmerId, cancellationToken);
    }
}
