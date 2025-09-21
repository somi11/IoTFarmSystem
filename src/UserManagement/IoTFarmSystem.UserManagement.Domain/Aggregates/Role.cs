using IoTFarmSystem.UserManagement.Domain.Entites;

public class Role
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }

    private readonly List<RolePermission> _permissions = new();
    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    private Role() { }
    public Role(string name) => Name = name;

    public void AddPermission(Permission permission) =>
        _permissions.Add(new RolePermission(this.Id, permission.Id));
}
