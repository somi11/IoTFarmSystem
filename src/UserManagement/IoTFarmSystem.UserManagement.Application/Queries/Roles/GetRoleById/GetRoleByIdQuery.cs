using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Roles.GetRoleById
{
    public record GetRoleByIdQuery(Guid RoleId) : IRequest<Role?>;
}
