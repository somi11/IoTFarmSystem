using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public List<string> Permissions { get; set; } = new(); // e.g. ["devices:read", "devices:create"]

        public string Description { get; set; } = string.Empty;
    }
}
