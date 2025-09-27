using IoTFarmSystem.Api.Dtos;
using IoTFarmSystem.Api.Extensions;
using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.SharedKernel.Security;
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
        private readonly ICurrentUserService _currentUser;

        public FarmersController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        // ----------------- COMMANDS -----------------

        [HttpPost("admin")]
        [Authorize(Policy = SystemPermissions.USERS_CREATE)]
        public async Task<IActionResult> CreateFarmerAsAdmin([FromBody] CreateFarmerByAdminDto dto)
        {
            var command = new CreateFarmerCommand(
                TenantId: dto.TenantId,
                Name: dto.Name,
                Email: dto.Email,
                Password: dto.Password,
                Roles: dto.Roles,
                Permissions: dto.Permissions,
                IsSelfSignUp: false
            );

            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetFarmerById), new { id = result.Value }, null);
        }

        [HttpPost("tenant")]
        [Authorize(Policy = SystemPermissions.USERS_CREATE)]
        public async Task<IActionResult> CreateFarmerAsTenant([FromBody] CreateFarmerByTenantDto dto)
        {
            var command = new CreateFarmerCommand(
                TenantId: _currentUser.TenantId!.Value,  // forced from claims
                Name: dto.Name,
                Email: dto.Email,
                Password: dto.Password,
                Roles: dto.Roles,
                Permissions: dto.Permissions,
                IsSelfSignUp: false
            );

            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetFarmerById), new { id = result.Value }, null);
        }
        //2 Self-signup — anonymous, tenant assigned automatically
        [HttpPost("sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpFarmerCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result.Error);
            return Ok(result.Value);
        }

        //3 Update existing farmer
        [HttpPut("{id:guid}")]
        [Authorize(Policy = SystemPermissions.USERS_UPDATE)]
        public async Task<IActionResult> UpdateFarmer(Guid id, [FromBody] UpdateFarmerCommand command)
        {
            if (id != command.FarmerId) return BadRequest("Farmer ID mismatch");

            var farmer = await _mediator.Send(new GetFarmerByIdQuery(id));
            if (farmer == null) return NotFound();

            return await this.ValidateTenantAccessAsync(
                _currentUser,
                farmer.TenantId,
                async () =>
                {
                    var result = await _mediator.Send(command);
                    if (!result.Success) return BadRequest(result.Error);
                    return NoContent();
                });
        }

        //4 Grant a permission
        [HttpPost("{id:guid}/permissions")]
        [Authorize(Policy = SystemPermissions.PERMISSIONS_ASSIGN)]
        public async Task<IActionResult> GrantPermissionToFarmer(Guid id, [FromBody] string permissionName)
        {
            var farmer = await _mediator.Send(new GetFarmerByIdQuery(id));
            if (farmer == null) return NotFound();

            return await this.ValidateTenantAccessAsync(
                _currentUser,
                farmer.TenantId,
                async () =>
                {
                    await _mediator.Send(new GrantPermissionToFarmerCommand(id, permissionName));
                    return NoContent();
                });
        }

        //5 Assign a role
        [HttpPost("{id:guid}/roles")]
        [Authorize(Policy = SystemPermissions.ROLES_ASSIGN)]
        public async Task<IActionResult> AssignRoleToFarmer(Guid id, [FromBody] string roleName)
        {
            var farmer = await _mediator.Send(new GetFarmerByIdQuery(id));
            if (farmer == null) return NotFound();

            return await this.ValidateTenantAccessAsync(
                _currentUser,
                farmer.TenantId,
                async () =>
                {
                    await _mediator.Send(new AssignRoleToFarmerCommand(id, roleName));
                    return NoContent();
                });
        }

        //6 Revoke a permission
        [HttpDelete("{id:guid}/permissions/{permissionName}")]
        [Authorize(Policy = SystemPermissions.PERMISSIONS_REVOKE)]
        public async Task<IActionResult> RevokePermissionFromFarmer(Guid id, string permissionName)
        {
            var farmer = await _mediator.Send(new GetFarmerByIdQuery(id));
            if (farmer == null) return NotFound();

            return await this.ValidateTenantAccessAsync(
                _currentUser,
                farmer.TenantId,
                async () =>
                {
                    await _mediator.Send(new RevokePermissionFromFarmerCommand(id, permissionName));
                    return NoContent();
                });
        }

        //7 Revoke a role
        [HttpDelete("{id:guid}/roles/{roleName}")]
        [Authorize(Policy = SystemPermissions.ROLES_ASSIGN)] // same policy for assigning & revoking
        public async Task<IActionResult> RevokeRoleFromFarmer(Guid id, string roleName)
        {
            var farmer = await _mediator.Send(new GetFarmerByIdQuery(id));
            if (farmer == null) return NotFound();

            return await this.ValidateTenantAccessAsync(
                _currentUser,
                farmer.TenantId,
                async () =>
                {
                    await _mediator.Send(new RevokeRoleFromFarmerCommand(id, roleName));
                    return NoContent();
                });
        }

        // ----------------- QUERIES -----------------

        //8 Get farmer by ID
        [HttpGet("{id:guid}")]
        [Authorize(Policy = SystemPermissions.USERS_READ)]
        public async Task<IActionResult> GetFarmerById(Guid id)
        {
            var farmer = await _mediator.Send(new GetFarmerByIdQuery(id));
            if (farmer == null) return NotFound();

            return await this.ValidateTenantAccessAsync(
                _currentUser,
                farmer.TenantId,
                async () => Ok(farmer));
        }

        //9 Get farmer by Email
        [HttpGet("by-email")]
        [Authorize(Policy = SystemPermissions.USERS_READ)]
        public async Task<IActionResult> GetFarmerByEmail([FromQuery] string email)
        {
            var farmer = await _mediator.Send(new GetFarmerByEmailQuery(email));
            if (farmer == null) return NotFound();

            return await this.ValidateTenantAccessAsync(
                _currentUser,
                farmer.TenantId,
                async () => Ok(farmer));
        }

        //10 Get farmers by role
        [HttpGet("tenants/{tenantId:guid}/farmers-by-role/{roleName}")]
        [Authorize(Policy = SystemPermissions.USERS_READ)]
        public async Task<IActionResult> GetFarmersByRole(Guid tenantId, string roleName)
        {
            return await this.ValidateTenantAccessAsync(
                _currentUser,
                tenantId,
                async () =>
                {
                    var farmers = await _mediator.Send(new GetFarmersByRoleQuery(tenantId, roleName));
                    return Ok(farmers);
                });
        }

        //11 Get farmers in tenant
        [HttpGet("tenants/{tenantId:guid}/farmers")]
        [Authorize(Policy = SystemPermissions.USERS_READ)]
        public async Task<IActionResult> GetFarmersByTenant(Guid tenantId)
        {
            return await this.ValidateTenantAccessAsync(
                _currentUser,
                tenantId,
                async () =>
                {
                    var farmers = await _mediator.Send(new GetFarmersByTenantQuery(tenantId));
                    return Ok(farmers);
                });
        }

        //12 Get farmer with roles
        [HttpGet("{id:guid}/roles")]
        [Authorize(Policy = SystemPermissions.ROLES_READ)]
        public async Task<IActionResult> GetFarmerWithRoles(Guid id)
        {
            var farmer = await _mediator.Send(new GetFarmerByIdQuery(id));
            if (farmer == null) return NotFound();

            return await this.ValidateTenantAccessAsync(
                _currentUser,
                farmer.TenantId,
                async () =>
                {
                    var data = await _mediator.Send(new GetFarmerWithRolesQuery(id));
                    return Ok(data);
                });
        }

        //13 Get farmer with permissions
        [HttpGet("{id:guid}/permissions")]
        [Authorize(Policy = SystemPermissions.PERMISSIONS_READ)]
        public async Task<IActionResult> GetFarmerWithPermissions(Guid id)
        {
            var farmer = await _mediator.Send(new GetFarmerByIdQuery(id));
            if (farmer == null) return NotFound();

            return await this.ValidateTenantAccessAsync(
                _currentUser,
                farmer.TenantId,
                async () =>
                {
                    var data = await _mediator.Send(new GetFarmerWithPermissionsQuery(id));
                    return Ok(data);
                });
        }
    }
}
