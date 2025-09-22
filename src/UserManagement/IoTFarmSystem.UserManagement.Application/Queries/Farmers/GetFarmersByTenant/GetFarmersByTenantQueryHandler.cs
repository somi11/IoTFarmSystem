using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmersByTenant
{
    public class GetFarmersByTenantQueryHandler : IRequestHandler<GetFarmersByTenantQuery, IReadOnlyList<Farmer>>
    {
        private readonly IFarmerRepository _farmerRepository;

        public GetFarmersByTenantQueryHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<IReadOnlyList<Farmer>> Handle(GetFarmersByTenantQuery request, CancellationToken cancellationToken) =>
            await _farmerRepository.GetByTenantAsync(request.TenantId, cancellationToken);
    }
}
