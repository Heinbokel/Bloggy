using Bloggy.Models;
using Bloggy.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bloggy.Services;

public class UserService {
    private readonly BloggyDbContext BloggyDbContext;

    public UserService(BloggyDbContext bloggyDbContext) {
        this.BloggyDbContext = bloggyDbContext;
    }

    public User RegisterUser(UserRegisterRequest request)
    {
        // Retrieve the UserRole included in the UserRegisterRequest and ensure it is valid.
        UserRole? requestedUserRole = this.RetrieveUserRoleById(request.UserRoleId);

        // If the requested UserRole exists, then we can continue in the process.
        if (requestedUserRole != null) {

            // Before we can register the new user, we need to hash/salt the password.
            // See the PasswordService methods for more details.
            byte[] passwordSalt = PasswordService.GenerateSalt();
            string hashedPassword = PasswordService.HashPassword(request.Password, passwordSalt);
            
            // Create the User object from the request and password hashing so we can save it to the database.
            User userToRegister = new User{
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
            // DbUpdateException is also thrown if the unique constraint on our UserName or Email is triggered.
            try {
                this.BloggyDbContext.Users.Add(userToRegister);
                this.BloggyDbContext.SaveChanges();
            } catch (DbUpdateException exception) {
                throw new Exception($"{exception.Message}", exception);
            }

            return userToRegister;
        }

        throw new Exception($"User Role {request.UserRoleId} does not exist.");
    }

    public UserRole? RetrieveUserRoleById(int requestedUserRoleId)
    {
        return this.BloggyDbContext.UserRoles.FirstOrDefault(userRole => userRole.Id == requestedUserRoleId);
    }


}