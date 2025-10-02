using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Roles.UpdateRole
{
    public record UpdateRoleCommand(Guid RoleId, string NewName) : IRequest<Unit>;
}
