using IoTFarmSystem.UserManagement.Application.Commands.Farmers.CreateFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.GrantPermissionToFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokePermissionFromFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokeRoleFromFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.UpdateFarmer;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerByEmail;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerById;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerWithPermissions;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerWithRoles;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmersByRole;
using IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmersByTenant;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTFarmSystem.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FarmersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FarmersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // CREATE farmer
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFarmerCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, null);
        }

        // GET farmer by Id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var farmer = await _mediator.Send(new GetFarmerByIdQuery(id));
            if (farmer == null) return NotFound();
            return Ok(farmer);
        }

        // GET farmer by Email
        [HttpGet("by-email")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var farmer = await _mediator.Send(new GetFarmerByEmailQuery(email));
            if (farmer == null) return NotFound();
            return Ok(farmer);
        }

        // GET farmers by Role
        [HttpGet("{tenantId}/farmers/by-role/{roleName}")]
        public async Task<IActionResult> GetByRole(Guid tenantId, string roleName)
        {
            var farmers = await _mediator.Send(new GetFarmersByRoleQuery(tenantId, roleName));
            return Ok(farmers);
        }

        // GET farmers by Tenant
        [HttpGet("by-tenant/{tenantId:guid}")]
        public async Task<IActionResult> GetByTenant(Guid tenantId)
        {
            var farmers = await _mediator.Send(new GetFarmersByTenantQuery(tenantId));
            return Ok(farmers);
        }

        // UPDATE farmer
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFarmerCommand command)
        {
            if (id != command.FarmerId)
                return BadRequest("Farmer ID mismatch");

            await _mediator.Send(command);
            return NoContent();
        }

        // GRANT permission
        [HttpPost("{id:guid}/permissions")]
        public async Task<IActionResult> GrantPermission(Guid id, [FromBody] string permissionName)
        {
            await _mediator.Send(new GrantPermissionToFarmerCommand(id, permissionName));
            return NoContent();
        }

        // REVOKE permission
        [HttpDelete("{id:guid}/permissions/{permissionName}")]
        public async Task<IActionResult> RevokePermission(Guid id, string permissionName)
        {
            await _mediator.Send(new RevokePermissionFromFarmerCommand(id, permissionName));
            return NoContent();
        }

        // REVOKE role
        [HttpDelete("{id:guid}/roles/{roleName}")]
        public async Task<IActionResult> RevokeRole(Guid id, string roleName)
        {
            await _mediator.Send(new RevokeRoleFromFarmerCommand(id, roleName));
            return NoContent();
        }

        // GET farmer with roles
        [HttpGet("{id:guid}/roles")]
        public async Task<IActionResult> GetWithRoles(Guid id)
        {
            var farmer = await _mediator.Send(new GetFarmerWithRolesQuery(id));
            if (farmer == null) return NotFound();
            return Ok(farmer);
        }

        // GET farmer with permissions
        [HttpGet("{id:guid}/permissions")]
        public async Task<IActionResult> GetWithPermissions(Guid id)
        {
            var farmer = await _mediator.Send(new GetFarmerWithPermissionsQuery(id));
            if (farmer == null) return NotFound();
            return Ok(farmer);
        }
    }
}
