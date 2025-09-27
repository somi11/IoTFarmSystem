using IoTFarmSystem.UserManagement.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Roles.AddPermissionToRole
{
    public record AddPermissionToRoleCommand(Guid RoleId, Permission Permission) : IRequest<Unit>;
}
