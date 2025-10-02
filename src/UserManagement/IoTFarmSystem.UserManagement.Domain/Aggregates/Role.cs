using IoTFarmSystem.UserManagement.Domain.Entites;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private readonly List<RolePermission> _permissions = new();
    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    private Role() { }
    public Role(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Role Id cannot be empty.", nameof(id));

        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
    public void AddPermission(Permission permission)
    {
        if (_permissions.Any(p => p.PermissionId == permission.Id)) return;
        _permissions.Add(new RolePermission(Id, permission.Id));
    }
}
