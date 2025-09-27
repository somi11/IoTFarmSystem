namespace IoTFarmSystem.UserManagement.Domain.Entites
{
    public class UserRole
    {
        public Guid UserId { get; init; }
        public Guid RoleId { get; init; }

        // Navigation property (not in constructor)
        public Role? Role { get; init; }

        // Parameterless constructor for EF Core
        private UserRole() { }

        // Public constructor for your own use
        public UserRole(Guid userId, Guid roleId ,Role role)
        {
            UserId = userId;
            RoleId = roleId;
            Role = role;
        }
    }
}
