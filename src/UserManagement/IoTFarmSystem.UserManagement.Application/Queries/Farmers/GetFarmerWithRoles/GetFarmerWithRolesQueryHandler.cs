using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerWithRoles
{
    public class GetFarmerWithRolesQueryHandler : IRequestHandler<GetFarmerWithRolesQuery, FarmerDto?>
    {
        private readonly IFarmerRepository _farmerRepository;

        public GetFarmerWithRolesQueryHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<FarmerDto?> Handle(GetFarmerWithRolesQuery request, CancellationToken cancellationToken) =>
            await _farmerRepository.GetWithRolesQueryAsync(request.FarmerId, cancellationToken);
    }
}
