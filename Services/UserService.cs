using Bloggy.Exceptions;
using Bloggy.Models;
using Bloggy.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bloggy.Services;

public class UserService
{
    private readonly BloggyDbContext BloggyDbContext;

    public UserService(BloggyDbContext bloggyDbContext)
    {
        this.BloggyDbContext = bloggyDbContext;
    }

    public User RegisterUser(UserRegisterRequest request)
    {
        
        // Retrieve the UserRole included in the UserRegisterRequest and ensure it is valid.
        UserRole? requestedUserRole = this.RetrieveUserRoleById(request.UserRoleId);

        // If the requested UserRole exists, then we can continue in the process.
        if (requestedUserRole != null)
        {

            // Before we can register the new user, we need to hash/salt the password.
            // See the PasswordService methods for more details.
            byte[] passwordSalt = PasswordService.GenerateSalt();
            string hashedPassword = PasswordService.HashPassword(request.Password, passwordSalt);

            // Create the User object from the request and password hashing so we can save it to the database.
            User userToRegister = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                UserRole = requestedUserRole,
                UserRoleId = request.UserRoleId,
                Salt = passwordSalt,
                Password = hashedPassword
            };

            // Attempt to save user to database.
            // Catch the DbUpdateException that will be thrown if anything goes wrong while saving the user to the database.
            // DbUpdateException is also thrown if the unique constraint on our UserName or Email is triggered, though I prefer to check manually.
            if (!this.UserAlreadyRegistered(userToRegister))
            {
                try
                {
                    this.BloggyDbContext.Users.Add(userToRegister);
                    this.BloggyDbContext.SaveChanges();
                }
                catch (DbUpdateException exception)
                {
                    throw new GeneralDatabaseException($"{exception.Message}", exception);
                }

                return userToRegister;
            }
            throw new UserAlreadyRegisteredException($"Username: {request.UserName} or Email: {request.Email} is unavailable.");

        }

        throw new EntityNotFoundException($"User Role {request.UserRoleId} could not be found.");
    }

    /// <summary>
    /// Returns true if the user is already registered.
    /// </summary>
    /// <param name="userToRegister">The user to look up registration status for.</param>
    /// <returns>True if the user is already registered.</returns>
    private bool UserAlreadyRegistered(User userToRegister)
    {
        return this.BloggyDbContext.Users.Any(user => user.Email == userToRegister.Email || user.UserName == userToRegister.UserName);
    }

    /// <summary>
    /// Returns the UserRole, or null if not found.
    /// </summary>
    /// <param name="requestedUserRoleId">The requested UserRole's ID.</param>
    /// <returns>The UserRole, or null if not found.</returns>
    public UserRole? RetrieveUserRoleById(int requestedUserRoleId)
    {
        return this.BloggyDbContext.UserRoles.FirstOrDefault(userRole => userRole.Id == requestedUserRoleId);
    }


}