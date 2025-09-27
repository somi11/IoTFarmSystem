using IoTFarmSystem.UserManagement.Application.Contracts.Authorizatioon;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using Microsoft.AspNetCore.Identity;

namespace IoTFarmSystem.UserManagement.Infrastructure.Identity
{
    public class AuthorizationClaimsProvider : IAuthorizationClaimsProvider
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFarmerRepository _farmerRepository;
        private readonly IRoleRepository _roleRepository;

        public AuthorizationClaimsProvider(
            UserManager<IdentityUser> userManager,
            IFarmerRepository farmerRepository,
            IRoleRepository roleRepository)
        {
            _userManager = userManager;
            _farmerRepository = farmerRepository;
            _roleRepository = roleRepository;
        }

        public async Task<IReadOnlyCollection<string>> GetPermissionsAsync(
            string identityUserId,
            CancellationToken ct = default)
        {
            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser == null)
                return Array.Empty<string>();

            // 1. Identity claims
            var identityClaims = await _userManager.GetClaimsAsync(identityUser);
            var permissions = identityClaims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            // 2. Domain (Farmer) permissions
            var farmer = await _farmerRepository.GetByIdentityUserIdAsync(identityUserId, ct);
            if (farmer != null)
            {
                permissions.AddRange(farmer.ExplicitPermissions.Select(p => p.PermissionName));
            }

            return permissions.Distinct().ToList();
        }

        public async Task<IReadOnlyCollection<string>> GetRolesAsync(
            string identityUserId,
            CancellationToken ct = default)
        {
            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser == null)
                return Array.Empty<string>();

            var roles = await _userManager.GetRolesAsync(identityUser);
            var roleNames = roles.ToList();

            // Merge in domain roles if farmer exists
            var farmer = await _farmerRepository.GetByIdentityUserIdAsync(identityUserId, ct);
            if (farmer != null)
            {
                foreach (var farmerRole in farmer.Roles)
                {
                    var role = await _roleRepository.GetByIdAsync(farmerRole.RoleId, ct);
                    if (role != null)
                        roleNames.Add(role.Name);
                }
            }

            return roleNames.Distinct().ToList();
        }

        public async Task<(Guid? FarmerId, Guid? TenantId, string? Email)> GetDomainIdentifiersAsync(
            string identityUserId,
            CancellationToken ct = default)
        {
            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser == null) return (null, null, null);

            var farmer = await _farmerRepository.GetByIdentityUserIdAsync(identityUserId, ct);
            if (farmer != null)
            {
                return (farmer.Id, farmer.TenantId, identityUser.Email);
            }

            return (null, null, identityUser.Email);
        }

        public async Task<IReadOnlyCollection<string>> GetEffectivePermissionsAsync(
            string identityUserId,
            CancellationToken ct = default)
        {
            var explicitPermissions = await GetPermissionsAsync(identityUserId, ct);
            var roles = await GetRolesAsync(identityUserId, ct);

            var allPermissions = new List<string>(explicitPermissions);

            foreach (var roleName in roles)
            {
                var role = await _roleRepository.GetByNameAsync(roleName, ct);
                if (role != null)
                {
                    allPermissions.AddRange(role.Permissions.Select(p => p.Permission.Name));
                }
            }

            return allPermissions.Distinct().ToList();
        }
    }
}
