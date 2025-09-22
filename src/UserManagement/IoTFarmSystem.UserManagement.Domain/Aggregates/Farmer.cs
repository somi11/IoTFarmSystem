
using IoTFarmSystem.UserManagement.Domain.Entites;
using System.Globalization;

public class Farmer
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string IdentityUserId { get; private set; }   // Foreign key to AspNetUsers

    public string Name { get; private set; }
    public string Email { get; private set; }
    public Guid TenantId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<UserRole> _roles = new();
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    private readonly List<UserPermission> _permissions = new();
    public IReadOnlyCollection<UserPermission> Permissions => _permissions.AsReadOnly();

    private Farmer() { }

    public Farmer(string identityUserId, string email, Guid tenantId)
    {
        IdentityUserId = identityUserId;
        Email = email;
        TenantId = tenantId;
    }

    public void AssignRole(Role role) =>
        _roles.Add(new UserRole(this.Id, role.Id ,role));

    public void RevokeRole(Role role)
    {
        var userRole = _roles.FirstOrDefault(ur => ur.RoleId == role.Id);
        if (userRole != null)
            _roles.Remove(userRole);
    }

    public void GrantPermission(Permission permission) =>
        _permissions.Add(new UserPermission(this.Id, permission.Id, permission.Name , permission));


    public void RevokePermission(string permissionName)
    {
        var userPermission = _permissions.FirstOrDefault(up => up.PermissionName == permissionName);
        if (userPermission != null)
            _permissions.Remove(userPermission);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        Name = name;
    }
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
