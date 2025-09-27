using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Domain.Entites
{
    public class Permission
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        private Permission() { } // EF Core

        public Permission(Guid id, string name)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Permission Id cannot be empty.", nameof(id));

            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
