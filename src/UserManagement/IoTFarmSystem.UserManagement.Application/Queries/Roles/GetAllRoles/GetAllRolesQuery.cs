using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Roles.GetAllRoles
{
    public record GetAllRolesQuery() : IRequest<IReadOnlyList<Role>>;
}
