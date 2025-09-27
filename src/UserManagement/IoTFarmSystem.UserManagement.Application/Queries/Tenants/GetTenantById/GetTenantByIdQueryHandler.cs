﻿using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Tenants.GetTenantById
{
    public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantDto?>
    {
        private readonly ITenantRepository _tenantRepository;

        public GetTenantByIdQueryHandler(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<TenantDto?> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
        {
            var tenant = await _tenantRepository.GetByIdQueryAsync(request.TenantId, cancellationToken);
            if (tenant is null) return null;
            return tenant;
            
        }
    }
}
