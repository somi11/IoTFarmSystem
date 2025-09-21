using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Contracts.Identity
{
    public interface IUserService
    {
        /// <summary>
        /// Creates a new Identity user with the given email and password.
        /// Returns the newly created user's Id.
        /// </summary>
        Task<string> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns a role to an existing user.
        /// </summary>
        Task AssignRoleAsync(string identityUserId, string roleName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user exists by email.
        /// </summary>
        Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Optionally, delete a user.
        /// </summary>
        Task DeleteUserAsync(string identityUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a role from an existing user.
        /// </summary>
        Task RemoveRoleAsync(string identityUserId, string roleName, CancellationToken cancellationToken = default);
    }
}
