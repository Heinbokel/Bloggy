using System.Data.Common;
using Bloggy.Exceptions;
using Bloggy.Models;
using Bloggy.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggy.Services;

/// <summary>
/// Service (Business Logic Layer) for working with the User entity.
/// </summary>
public class UserService
{
    private readonly BloggyDbContext BloggyDbContext;
    private readonly JwtService JwtService;

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="bloggyDbContext">The BloggyDbContext to provide to this class.</param>
    /// <param name="jwtService">The JwtService to provide to this class.</param>
    public UserService(BloggyDbContext bloggyDbContext, JwtService jwtService)
    {
        this.BloggyDbContext = bloggyDbContext;
        this.JwtService = jwtService;
    }

    /// <summary>
    /// Registers a new user in Bloggy's database provided the user does not already exist.
    /// </summary>
    /// <param name="request">The UserRegisterRequest to register the user from.</param>
    /// <returns>The created User to return.</returns>
    public User RegisterUser(UserRegisterRequest request)
    {
        // Check to see if the username or email is already registered before proceeding.
        if (this.UserAlreadyRegistered(request.Email, request.UserName))
        {
            throw new UserAlreadyRegisteredException($"Username: {request.UserName} or Email: {request.Email} is unavailable.");
        }

        // Retrieve the UserRole included in the UserRegisterRequest and ensure it is valid.
        UserRole? requestedUserRole = this.RetrieveUserRoleById(request.UserRoleId);

        // We need to ensure the requested user role is actually available.
        if (requestedUserRole == null) {
            throw new EntityNotFoundException($"User role with ID: {request.UserRoleId} was not found.");
        }

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
        try
        {
            this.BloggyDbContext.Users.Add(userToRegister);
            this.BloggyDbContext.SaveChanges();
            return userToRegister;
        }
        catch (DbUpdateException exception)
        {
            throw new GeneralDatabaseException($"{exception.Message}", exception);
        }
    }

    /// <summary>
    /// Attempts to authenticate a user and returns a response containing a JWT if successful.
    /// </summary>
    /// <param name="request">The LoginRequest to authenticate.</param>
    /// <returns>The LoginResponse containing a JWT to return.</returns>
    [HttpPost("Login", Name = "Login")]
    public LoginResponse Login(LoginRequest request)
    {
        // Attempt to retrieve the user associated with the sent in email.
        User? userToAuthenticate = this.RetrieveUserByEmail(request.Email);

        // If the user is not found, just tell the user the credentials were invalid.
        // We throw our custom InvalidLoginException and the frontend should tell the user
        // that the credentials were invalid. Not that the user wasn't found, because we don't want
        // a bad actor to know what accounts exist and don't exist.
        if (userToAuthenticate == null)
        {
            throw new InvalidLoginException($"Credentials were invalid.");
        }
        else
        {
            // If we are here, it means a user with the given email exists.
            // We now need to verify the login request's password against the one in the database.
            // But we need to salt and hash the login request's password the same exact way we did when
            // the user created the account, so that we are comparing apples to apples.
            // We should never be able to know what a user's password is, even the passwords in our database, 
            // so we can't simply dehash the stored password.
            string hashedRequestPassword = PasswordService.HashPassword(request.Password, userToAuthenticate.Salt);
            if (hashedRequestPassword == userToAuthenticate.Password)
            {
                // If we are here, it means the user passed in the correct password. Now we must generate the 
                // JWT (JSON Web Token) containing their user details to send back in the HTTP Response.
                string jwt = this.JwtService.GenerateJwtSecurityTokenFromUser(userToAuthenticate);
                return new LoginResponse { Token = jwt };
            }
            else
            {
                // If we are here, the user entered an email that exists, but the incorrect password.
                // So we will just throw our InvalidLoginException stating a generic error phrase.
                throw new InvalidLoginException("Credentials were invalid.");
            }
        }
    }

    public User? RetrieveUserById(int id)
    {
        try {
            return this.BloggyDbContext.Users
            .Include(user => user.UserRole)
            .FirstOrDefault(user => user.Id == id);
        } catch (DbException exception) {
            throw new GeneralDatabaseException($"Error occurred retrieving user with ID {id}", exception);
        }
    }

    private User? RetrieveUserByEmail(string email)
    {
        try {
            return this.BloggyDbContext.Users
            .Include(user => user.UserRole)
            .FirstOrDefault(user => user.Email.ToUpper() == email.ToUpper());
        } catch (DbException exception) {
            throw new GeneralDatabaseException($"Error occurred retrieving user with email {email}", exception);
        }
    }

    /// <summary>
    /// Returns true if the user is already registered.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <param name="username">The username to check.</param>
    /// <returns>True if the user is already registered.</returns>
    private bool UserAlreadyRegistered(string email, string username)
    {
        try {
            return this.BloggyDbContext.Users.Any(user => user.Email == email || user.UserName == username);
        } catch (DbException exception) {
            throw new GeneralDatabaseException($"{exception.Message}", exception);
        }
    }

    /// <summary>
    /// Returns the UserRole, or null if not found.
    /// </summary>
    /// <param name="requestedUserRoleId">The requested UserRole's ID.</param>
    /// <returns>The UserRole, or null if not found.</returns>
    public UserRole? RetrieveUserRoleById(int requestedUserRoleId)
    {
        try {
            return this.BloggyDbContext.UserRoles.FirstOrDefault(userRole => userRole.Id == requestedUserRoleId);
        } catch (DbException exception) {
            throw new GeneralDatabaseException($"{exception.Message}", exception);
        }
    }


}