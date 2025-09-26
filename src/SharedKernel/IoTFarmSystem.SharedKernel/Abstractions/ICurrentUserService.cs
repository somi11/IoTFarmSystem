using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.SharedKernel.Abstractions
{
    public interface ICurrentUserService
    {
        Guid? FarmerId { get; }
        Guid? TenantId { get; }
        string? IdentityUserId { get; }
        bool HasClaim(string type, string value);
        //Host implements this by reading claims from HttpContext.
    }
}
