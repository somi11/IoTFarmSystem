using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmersByTenant
{
    public class GetFarmersByTenantQueryHandler : IRequestHandler<GetFarmersByTenantQuery, IReadOnlyList<FarmerDto>>
    {
        private readonly IFarmerRepository _farmerRepository;

        public GetFarmersByTenantQueryHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<IReadOnlyList<FarmerDto>> Handle(GetFarmersByTenantQuery request, CancellationToken cancellationToken) =>
            await _farmerRepository.GetByTenantAsync(request.TenantId, cancellationToken);
    }
}
