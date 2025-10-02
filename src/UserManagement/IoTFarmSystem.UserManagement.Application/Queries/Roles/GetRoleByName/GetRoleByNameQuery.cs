using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Roles.GetRoleByName
{
    public record GetRoleByNameQuery(string RoleName) : IRequest<Role?>;
}
