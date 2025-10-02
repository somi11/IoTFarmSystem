using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Contracts.Identity
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(string identityUserId, CancellationToken cancellationToken = default);
    }
}
