using IoTFarmSystem.UserManagement.Application.Commands.Tenants.CreateTenantCommand;
using IoTFarmSystem.UserManagement.Application.Commands.Tenants.DeleteTenant;
using IoTFarmSystem.UserManagement.Application.Commands.Tenants.UpdateTenant;
using IoTFarmSystem.UserManagement.Application.DTOs;
using IoTFarmSystem.UserManagement.Application.Queries.Tenants.GetAllTenants;
using IoTFarmSystem.UserManagement.Application.Queries.Tenants.GetTenantById;
using IoTFarmSystem.UserManagement.Application.Queries.Tenants.GetTenantByName;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IoTFarmSystem.Host.Controllers
{
    [ApiController]
    [Route("api/tenants")]
    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ----------------- QUERIES -----------------

        // Get all tenants
        [HttpGet]
        public async Task<IActionResult> GetAllTenants(CancellationToken cancellationToken)
        {
            var tenants = await _mediator.Send(new GetAllTenantsQuery(), cancellationToken);
            return Ok(tenants);
        }

        // Get tenant by Id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTenantById(Guid id, CancellationToken cancellationToken)
        {
            var tenant = await _mediator.Send(new GetTenantByIdQuery(id), cancellationToken);
            if (tenant == null) return NotFound();
            return Ok(tenant);
        }

        // Get tenant by name
        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetTenantByName(string name, CancellationToken cancellationToken)
        {
            var tenant = await _mediator.Send(new GetTenantByNameQuery(name), cancellationToken);
            if (tenant == null) return NotFound();
            return Ok(tenant);
        }

        // ----------------- COMMANDS -----------------

        // Create tenant
        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetTenantById), new { id = result.Value }, null);
        }

        // Update tenant
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] UpdateTenantCommand command, CancellationToken cancellationToken)
        {
            if (id != command.TenantId)
                return BadRequest("Tenant ID mismatch");

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        // Delete tenant
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTenant(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteTenantCommand(id), cancellationToken);
            return NoContent();
        }
    }
}
