using IoTFarmSystem.UserManagement.Application.Commands.Farmers.AssignRoleToFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.CreateFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.GrantPermissionToFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokePermissionFromFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokeRoleFromFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.SignUpFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.UpdateFarmer;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerByEmail;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerById;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmersByRole;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmersByTenant;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerWithPermissions;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerWithRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoTFarmSystem.Host.Controllers
{
    [ApiController]
    [Route("api/farmers")]
    public class FarmersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FarmersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ----------------- COMMANDS -----------------

        // Create new farmer
        [HttpPost]
        public async Task<IActionResult> CreateFarmer([FromBody] CreateFarmerCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetFarmerById), new { id = result.Value }, null);
        }
        [HttpPost("sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpFarmerCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result.Error);
            return Ok(result.Value); // returns FarmerId
        }
        // Update existing farmer
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateFarmer(Guid id, [FromBody] UpdateFarmerCommand command)
        {
            if (id != command.FarmerId)
                return BadRequest("Farmer ID mismatch");

            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result.Error);

            return NoContent();
        }

        // Grant a specific permission to farmer
        [HttpPost("{id:guid}/permissions")]
        public async Task<IActionResult> GrantPermissionToFarmer(Guid id, [FromBody] string permissionName)
        {
            await _mediator.Send(new GrantPermissionToFarmerCommand(id, permissionName));
            return NoContent();
        }
        // Grant a specific role to farmer
        [HttpPost("{id:guid}/roles")]
        public async Task<IActionResult> AssignRoleToFarmer(Guid id, [FromBody] string roleName)
        {
            await _mediator.Send(new AssignRoleToFarmerCommand(id, roleName));
            return NoContent();
        }

        // Revoke a specific permission from farmer
        [HttpDelete("{id:guid}/permissions/{permissionName}")]
        public async Task<IActionResult> RevokePermissionFromFarmer(Guid id, string permissionName)
        {
            await _mediator.Send(new RevokePermissionFromFarmerCommand(id, permissionName));
            return NoContent();
        }

        // Revoke a role from farmer
        [HttpDelete("{id:guid}/roles/{roleName}")]
        public async Task<IActionResult> RevokeRoleFromFarmer(Guid id, string roleName)
        {
            await _mediator.Send(new RevokeRoleFromFarmerCommand(id, roleName));
            return NoContent();
        }

        // ----------------- QUERIES -----------------

        // Get farmer by ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetFarmerById(Guid id)
        {
            var farmer = await _mediator.Send(new GetFarmerByIdQuery(id));
            if (farmer == null) return NotFound();
            return Ok(farmer);
        }

        // Get farmer by Email
        [HttpGet("by-email")]
        public async Task<IActionResult> GetFarmerByEmail([FromQuery] string email)
        {
            var farmer = await _mediator.Send(new GetFarmerByEmailQuery(email));
            if (farmer == null) return NotFound();
            return Ok(farmer);
        }

        // Get all farmers with given role in tenant
        [HttpGet("tenants/{tenantId:guid}/farmers-by-role/{roleName}")]
        public async Task<IActionResult> GetFarmersByRole(Guid tenantId, string roleName)
        {
            var farmers = await _mediator.Send(new GetFarmersByRoleQuery(tenantId, roleName));
            return Ok(farmers);
        }

        // Get all farmers in a tenant
        [HttpGet("tenants/{tenantId:guid}/farmers")]
        public async Task<IActionResult> GetFarmersByTenant(Guid tenantId)
        {
            var farmers = await _mediator.Send(new GetFarmersByTenantQuery(tenantId));
            return Ok(farmers);
        }

        // Get farmer with roles
        [HttpGet("{id:guid}/roles")]
        public async Task<IActionResult> GetFarmerWithRoles(Guid id)
        {
            var farmer = await _mediator.Send(new GetFarmerWithRolesQuery(id));
            if (farmer == null) return NotFound();
            return Ok(farmer);
        }

        // Get farmer with permissions
        [HttpGet("{id:guid}/permissions")]
        public async Task<IActionResult> GetFarmerWithPermissions(Guid id)
        {
            var farmer = await _mediator.Send(new GetFarmerWithPermissionsQuery(id));
            if (farmer == null) return NotFound();
            return Ok(farmer);
        }
    }
}
