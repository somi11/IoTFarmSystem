using IoTFarmSystem.UserManagement.Application.Commands.Tenants.CreateTenantCommand;
using IoTFarmSystem.UserManagement.Application.Commands.Tenants.DeleteTenant;
using IoTFarmSystem.UserManagement.Application.Commands.Tenants.UpdateTenant;
using IoTFarmSystem.UserManagement.Application.Queries.Tenants.GetAllTenants;
using IoTFarmSystem.UserManagement.Application.Queries.Tenants.GetTenantById;
using IoTFarmSystem.UserManagement.Application.Queries.Tenants.GetTenantByName;
using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.SharedKernel.Security;
using IoTFarmSystem.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoTFarmSystem.Host.Controllers
{
    [ApiController]
    [Route("api/tenants")]
    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public TenantsController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        // ----------------- QUERIES -----------------

        //1 Get all tenants
        [HttpGet]
        [Authorize(Policy = SystemPermissions.TENANTS_READ)]
        public async Task<IActionResult> GetAllTenants(CancellationToken cancellationToken)
        {
            // System admins see all tenants
            if (_currentUser.IsSystemAdmin())
            {
                var tenants = await _mediator.Send(new GetAllTenantsQuery(), cancellationToken);
                return Ok(tenants);
            }

            // Tenant-scoped users only see their tenant
            if (_currentUser.TenantId.HasValue)
            {
                var userTenant = await _mediator.Send(new GetTenantByIdQuery(_currentUser.TenantId.Value), cancellationToken);
                return Ok(new[] { userTenant });
            }

            return Forbid();
        }

        //2 Get tenant by ID
        [HttpGet("{id:guid}")]
        [Authorize(Policy = SystemPermissions.TENANTS_READ)]
        public async Task<IActionResult> GetTenantById(Guid id, CancellationToken cancellationToken)
        {
            return await this.ValidateTenantAccessAsync(_currentUser, id, async () =>
            {
                var tenant = await _mediator.Send(new GetTenantByIdQuery(id), cancellationToken);
                if (tenant == null) return NotFound();
                return Ok(tenant);
            });
        }

        //3 Get tenant by Name
        [HttpGet("by-name/{name}")]
        [Authorize(Policy = SystemPermissions.TENANTS_READ)]
        public async Task<IActionResult> GetTenantByName(string name, CancellationToken cancellationToken)
        {
            var tenant = await _mediator.Send(new GetTenantByNameQuery(name), cancellationToken);
            if (tenant == null) return NotFound();

            // Non-system admins can only see their own tenant
            if (!_currentUser.IsSystemAdmin() &&
                _currentUser.TenantId.HasValue &&
                tenant.Id != _currentUser.TenantId.Value)
            {
                return Forbid();
            }

            return Ok(tenant);
        }

        // ----------------- COMMANDS -----------------

        //4 Create tenant
        [HttpPost]
        [Authorize(Policy = SystemPermissions.TENANTS_CREATE)]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.Success) return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetTenantById), new { id = result.Value }, null);
        }

        //5 Update tenant
        [HttpPut("{id:guid}")]
        [Authorize(Policy = SystemPermissions.TENANTS_UPDATE)]
        public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] UpdateTenantCommand command, CancellationToken cancellationToken)
        {
            if (id != command.TenantId) return BadRequest("Tenant ID mismatch");

            return await this.ValidateTenantAccessAsync(_currentUser, id, async () =>
            {
                var result = await _mediator.Send(command, cancellationToken);
                return result.Success ? NoContent() : BadRequest(result.Error);
            });
        }

        //6 Delete tenant
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = SystemPermissions.TENANTS_DELETE)]
        public async Task<IActionResult> DeleteTenant(Guid id, CancellationToken cancellationToken)
        {
            return await this.ValidateTenantAccessAsync(_currentUser, id, async () =>
            {
                await _mediator.Send(new DeleteTenantCommand(id), cancellationToken);
                return NoContent();
            });
        }
    }
}
