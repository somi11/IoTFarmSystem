using IoTFarmSystem.SharedKernel.Security;

public class Tenant
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private readonly List<Farmer> _farmers = new();
    public IReadOnlyCollection<Farmer> Farmers => _farmers.AsReadOnly();

    private Tenant() { } // EF Core needs this

    // Constructor for creating new Tenant
    public Tenant(Guid id, string name)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty", nameof(id));
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Farmer RegisterFarmer(Guid farmerid, string identityUserId, string email, string name)
    {
        var farmer = new Farmer(farmerid, identityUserId, email, Id, name);
        _farmers.Add(farmer);
        return farmer;
    }

    public bool HasOwner() =>
        _farmers.Any(f => f.Roles.Any(r => r.Role.Name == SystemRoles.TENANT_OWNER));

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Name = name;
    }
}
