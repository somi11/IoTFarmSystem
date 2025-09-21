using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Domain.Entites
{
    public class Permission
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }

        // Fix: Ensure 'Name' is initialized for non-nullable property
        private Permission()
        {
            Name = string.Empty;
        }
        public Permission(string name) => Name = name;
    }
}
