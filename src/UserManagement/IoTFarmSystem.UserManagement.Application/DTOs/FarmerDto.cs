using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.DTOs
{
    public class FarmerDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;

        public List<string> Roles { get; set; } = new(); // e.g. ["FarmerManager", "Viewer"]
        public List<string> Permissions { get; set; } = new(); // Flattened from roles

        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
