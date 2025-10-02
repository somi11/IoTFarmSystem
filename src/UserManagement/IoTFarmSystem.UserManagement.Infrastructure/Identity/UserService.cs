using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using Microsoft.AspNetCore.Identity;

namespace IoTFarmSystem.UserManagement.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = new IdentityUser { UserName = email, Email = email };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return user.Id;
        }

        public async Task AssignRoleAsync(string identityUserId, string roleName, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(identityUserId)
                       ?? throw new KeyNotFoundException($"User '{identityUserId}' not found");

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                // Auto-create the role if it doesn’t exist
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                    throw new InvalidOperationException(
                        $"Failed to create role '{roleName}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task DeleteUserAsync(string identityUserId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(identityUserId)
                       ?? throw new KeyNotFoundException($"User '{identityUserId}' not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        public async Task RemoveRoleAsync(string identityUserId, string roleName, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(identityUserId)
                       ?? throw new KeyNotFoundException($"User '{identityUserId}' not found");

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to remove role '{roleName}' from user '{identityUserId}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        public async Task UpdateEmailAsync(string identityUserId, string newEmail, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(identityUserId)
                       ?? throw new KeyNotFoundException($"User '{identityUserId}' not found");

            // Optional: check if new email already exists
            var existingUser = await _userManager.FindByEmailAsync(newEmail);
            if (existingUser != null && existingUser.Id != user.Id)
                throw new InvalidOperationException($"Email '{newEmail}' is already taken.");

            user.Email = newEmail;
            user.UserName = newEmail; 

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to update email: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
