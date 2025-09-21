using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Tenants.GetAllTenants
{
    public record GetAllTenantsQuery() : IRequest<IReadOnlyList<TenantDto>>;
}
