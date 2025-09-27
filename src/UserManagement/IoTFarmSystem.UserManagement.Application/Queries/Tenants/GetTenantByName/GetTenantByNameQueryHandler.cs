using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Tenants.GetTenantByName
{
    public class GetTenantByNameQueryHandler : IRequestHandler<GetTenantByNameQuery, TenantDto?>
    {
        private readonly ITenantRepository _tenantRepository;

        public GetTenantByNameQueryHandler(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<TenantDto?> Handle(GetTenantByNameQuery request, CancellationToken cancellationToken)
        {
            var tenant = await _tenantRepository.GetByNameQueryAsync(request.Name, cancellationToken);
            return tenant;
        }
    }
}
