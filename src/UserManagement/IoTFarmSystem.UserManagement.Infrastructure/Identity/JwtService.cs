using IoTFarmSystem.UserManagement.Application.Contracts.Authorizatioon;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Domain.Entites;
using IoTFarmSystem.UserManagement.Infrastructure.Persistance.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IoTFarmSystem.UserManagement.Infrastructure.Identity
{
    public class JwtService : IJwtService
    {
        private readonly IAuthorizationClaimsProvider _claimsProvider;
        private readonly JwtSettings _settings;
        private readonly ILogger<JwtService> _logger;

        public JwtService(
            IAuthorizationClaimsProvider claimsProvider,
            IOptions<JwtSettings> options,
            ILogger<JwtService> logger)
        {
            _claimsProvider = claimsProvider;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<string> GenerateTokenAsync(
            string identityUserId,
            CancellationToken cancellationToken = default)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, identityUserId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

            try
            {
                // Add domain identifiers
                var ids = await _claimsProvider.GetDomainIdentifiersAsync(identityUserId, cancellationToken);
                if (!string.IsNullOrEmpty(ids.Email))
                    claims.Add(new Claim(JwtRegisteredClaimNames.Email, ids.Email));
                if (ids.FarmerId.HasValue)
                    claims.Add(new Claim("farmerId", ids.FarmerId.ToString()!));
                if (ids.TenantId.HasValue)
                    claims.Add(new Claim("tenantId", ids.TenantId.ToString()!));

                // Add roles
                var roles = await _claimsProvider.GetRolesAsync(identityUserId, cancellationToken);
                foreach (var role in roles)
                {
                    claims.Add(new Claim("role", role));
                }

                // Add effective permissions (includes both explicit and role-based)
                var permissions = await _claimsProvider.GetEffectivePermissionsAsync(identityUserId, cancellationToken);
                foreach (var perm in permissions)
                    claims.Add(new Claim("permission", perm));

                _logger.LogDebug("Generated token for user {UserId} with {RoleCount} roles and {PermissionCount} permissions",
                    identityUserId, roles.Count, permissions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving claims for user {UserId}", identityUserId);
                throw;
            }

            // Build token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //public class JwtService : IJwtService
        //{
        //    private readonly IAuthorizationClaimsProvider _claimsProvider;
        //    private readonly JwtSettings _settings;

        //    public JwtService(
        //        IAuthorizationClaimsProvider claimsProvider,
        //        IOptions<JwtSettings> options)
        //    {
        //        _claimsProvider = claimsProvider;
        //        _settings = options.Value;
        //    }

        //public async Task<string> GenerateTokenAsync(
        //        string identityUserId,
        //        CancellationToken cancellationToken = default)
        //    {
        //        var claims = new List<Claim>
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, identityUserId),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    };

        //        // --- Add FarmerId / TenantId / Email ---
        //        var ids = await _claimsProvider.GetDomainIdentifiersAsync(identityUserId, cancellationToken);
        //        if (ids.Email is not null)
        //            claims.Add(new Claim(JwtRegisteredClaimNames.Email, ids.Email));
        //        if (ids.FarmerId.HasValue)
        //            claims.Add(new Claim("farmerId", ids.FarmerId.ToString()!));
        //        if (ids.TenantId.HasValue)
        //            claims.Add(new Claim("tenantId", ids.TenantId.ToString()!));

        //        // --- Add Roles ---
        //        var roles = await _claimsProvider.GetRolesAsync(identityUserId, cancellationToken);
        //        foreach (var role in roles)
        //            claims.Add(new Claim("role", role)); // multiple roles = array in JWT

        //        // --- Add Permissions ---
        //        var permissions = await _claimsProvider.GetEffectivePermissionsAsync(identityUserId, cancellationToken);
        //        foreach (var perm in permissions)
        //            claims.Add(new Claim("permission", perm));

        //        // --- Build token ---
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var token = new JwtSecurityToken(
        //            issuer: _settings.Issuer,
        //            audience: _settings.Audience,
        //            claims: claims,
        //            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
        //            signingCredentials: creds);

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    }
        //}

        //public class JwtService : IJwtService
        //{
        //    private readonly UserManager<IdentityUser> _userManager;
        //    private readonly IFarmerRepository _farmerRepository;
        //    private readonly IRoleRepository _roleRepository;
        //    private readonly JwtSettings _settings;

        //    public JwtService(
        //        UserManager<IdentityUser> userManager,
        //        IFarmerRepository farmerRepository,
        //        IRoleRepository roleRepository,
        //        IOptions<JwtSettings> options)
        //    {
        //        _userManager = userManager;
        //        _farmerRepository = farmerRepository;
        //        _roleRepository = roleRepository;
        //        _settings = options.Value;
        //    }

        //    public async Task<string> GenerateTokenAsync(string identityUserId, CancellationToken cancellationToken = default)
        //    {
        //        // 1. Load Identity user
        //        var identityUser = await _userManager.FindByIdAsync(identityUserId)
        //            ?? throw new KeyNotFoundException($"Identity user '{identityUserId}' not found");

        //        // 2. Load Farmer aggregate (domain user)
        //        var farmer = await _farmerRepository.GetByIdentityUserIdAsync(identityUserId, cancellationToken);

        //        // 3. Collect Identity roles
        //        var identityRoles = await _userManager.GetRolesAsync(identityUser);

        //        // 4. Build claims
        //        var claims = new List<Claim>
        //        {
        //            new Claim(JwtRegisteredClaimNames.Sub, identityUser.Id),
        //            new Claim(JwtRegisteredClaimNames.Email, identityUser.Email ?? ""),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //        };

        //        if (farmer != null)
        //        {
        //            if (!farmer.IsActive)
        //                throw new UnauthorizedAccessException("Farmer is inactive.");

        //            claims.Add(new Claim("farmerId", farmer.Id.ToString()));
        //            claims.Add(new Claim("tenantId", farmer.TenantId.ToString()));

        //            // Domain roles & permissions
        //            foreach (var userRole in farmer.Roles)
        //            {
        //                var role = await _roleRepository.GetByIdAsync(userRole.RoleId, cancellationToken);
        //                if (role != null)
        //                {
        //                    // role name
        //                    claims.Add(new Claim("role", role.Name));
        //                }
        //            }

        //            // Permissions (still as names)
        //            var domainPermissions = farmer.ExplicitPermissions.Select(p => p.PermissionName).ToList();
        //            claims.AddRange(domainPermissions.Select(p => new Claim("permission", p)));
        //        }
        //        else
        //        {
        //            // Identity-only user (e.g., SystemAdmin)
        //            // Get all claims from Identity
        //            var identityClaims = await _userManager.GetClaimsAsync(identityUser);
        //            claims.AddRange(identityClaims);
        //        }

        //        // Identity roles as claims
        //        claims.AddRange(identityRoles.Select(r => new Claim("role", r)));
        //        // 5. Create JWT
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var token = new JwtSecurityToken(
        //            issuer: _settings.Issuer,
        //            audience: _settings.Audience,
        //            claims: claims,
        //            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
        //            signingCredentials: creds);

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    }

        //}

    }
}
