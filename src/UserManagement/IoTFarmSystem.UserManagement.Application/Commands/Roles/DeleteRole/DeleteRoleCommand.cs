using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Roles.DeleteRole
{
    public record DeleteRoleCommand(Guid RoleId) : IRequest<Unit>;
}
