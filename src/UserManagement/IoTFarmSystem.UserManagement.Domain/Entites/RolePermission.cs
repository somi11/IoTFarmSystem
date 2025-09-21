using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Domain.Entites
{
    public record RolePermission(Guid RoleId, Guid PermissionId);

}
