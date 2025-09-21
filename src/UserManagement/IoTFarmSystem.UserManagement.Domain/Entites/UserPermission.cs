using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Domain.Entites
{
    public record UserPermission(Guid UserId, Guid PermissionId);
}
