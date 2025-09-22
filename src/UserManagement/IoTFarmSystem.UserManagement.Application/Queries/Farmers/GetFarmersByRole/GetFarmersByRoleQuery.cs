using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmersByRole
{
    public record GetFarmersByRoleQuery(string RoleName) : IRequest<IReadOnlyList<Farmer>>;
}
