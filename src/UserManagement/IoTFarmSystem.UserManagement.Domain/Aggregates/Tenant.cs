public class Tenant
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }

    private readonly List<Farmer> _farmers = new();
    public IReadOnlyCollection<Farmer> Farmers => _farmers.AsReadOnly();

    private Tenant() { }

    public Tenant(string name) => Name = name;

    public Farmer RegisterFarmer(string identityUserId, string email)
    {
        var farmer = new Farmer(identityUserId, email, this.Id);
        _farmers.Add(farmer);
        return farmer;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
}
