using IoTFarmSystem.UserManagement.Domain.Entites;

public class Farmer
{
    public Guid Id { get; private set; }
    public string IdentityUserId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public Guid TenantId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<UserRole> _roles = new();
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    // Stores only explicit permissions granted directly to the farmer
    private readonly List<UserPermission> _permissions = new();
    public IReadOnlyCollection<UserPermission> ExplicitPermissions => _permissions.AsReadOnly();

    private Farmer() { } // EF Core

    public Farmer(Guid id, string identityUserId, string email, Guid tenantId, string name)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty", nameof(id));
        Id = id;
        IdentityUserId = identityUserId ?? throw new ArgumentNullException(nameof(identityUserId));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        TenantId = tenantId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    // ---------- Role management ----------
    public void AssignRole(Role role)
    {
        if (_roles.Any(r => r.RoleId == role.Id)) return;
        _roles.Add(new UserRole(Id, role.Id, role));
    }

    public void RevokeRole(Role role)
    {
        var userRole = _roles.FirstOrDefault(ur => ur.RoleId == role.Id);
        if (userRole != null) _roles.Remove(userRole);
    }

    public void RevokeRoleWithPermissions(Role role)
    {
        // Remove the role
        var userRole = _roles.FirstOrDefault(r => r.RoleId == role.Id);
        if (userRole != null)
            _roles.Remove(userRole);

    }

    // ---------- Explicit permission management ----------
    public void GrantPermission(Permission permission)
    {
        if (_permissions.Any(p => p.PermissionId == permission.Id))
            return; // already explicitly granted

        _permissions.Add(new UserPermission(Id, permission.Id, permission.Name, null));
    }

    public void RevokePermission(Guid permissionId)
    {
        var userPermission = _permissions.FirstOrDefault(up => up.PermissionId == permissionId);
        if (userPermission != null)
            _permissions.Remove(userPermission);
    }

    // ---------- Effective permissions (roles + explicit) ----------
    public IReadOnlyCollection<Permission> GetEffectivePermissions()
    {
        var rolePermissions = _roles
            .SelectMany(r => r.Role.Permissions)
            .Select(rp => new Permission(rp.PermissionId, rp.Permission.Name));

        var explicitPermissions = _permissions
            .Select(up => new Permission(up.PermissionId, up.PermissionName));

        return rolePermissions
            .Concat(explicitPermissions)
            .DistinctBy(p => p.Id)
            .ToList();
    }

    // ---------- Profile management ----------
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        Name = name;
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        Email = email;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
