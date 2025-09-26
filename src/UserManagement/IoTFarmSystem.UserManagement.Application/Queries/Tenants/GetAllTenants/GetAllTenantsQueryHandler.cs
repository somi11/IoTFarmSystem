using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Tenants.GetAllTenants
{
    public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, IReadOnlyList<TenantDto>>
    {
        private readonly ITenantRepository _tenantRepository;

        public GetAllTenantsQueryHandler(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<IReadOnlyList<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
        {
            var tenants = await _tenantRepository.GetAllWithFarmersQueryAsync(cancellationToken);

            return tenants;
        }
    }
}
