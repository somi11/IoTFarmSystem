using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmersByRole
{
    public record GetFarmersByRoleQuery(Guid TenantId, string RoleName) : IRequest<IReadOnlyList<FarmerDto>>;
}
