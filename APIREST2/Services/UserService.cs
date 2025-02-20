using APIREST2.Models;
using APIREST2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace APIREST2.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                _logger.LogInformation("Retrieved {Count} users", users.Count);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                throw;
            }
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>The created user.</returns>
        /// <exception cref="InvalidOperationException">Thrown when there is an error creating the user.</exception>
        public async Task<User> CreateUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created new user with ID {Id}", user.Id);
                return user;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw new InvalidOperationException("Error creating user. The email might already exist.", ex);
            }
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>True if the user was deleted, false if the user was not found.</returns>
        /// <exception cref="Exception">Thrown when there is an error deleting the user.</exception>
        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {Id} not found", id);
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted user with ID {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Checks if a user exists by ID.
        /// </summary>
        /// <param name="id">The ID of the user to check.</param>
        /// <returns>True if the user exists, false otherwise.</returns>
        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }
    }
}