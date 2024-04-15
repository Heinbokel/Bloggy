using Bloggy.Exceptions;
using Bloggy.Models;
using Bloggy.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Controllers;

[ApiController]
public class UsersController: ControllerBase {
    private readonly UserService UserService;

    public UsersController(UserService userService) {
        this.UserService = userService;
    }

    /// <summary>
    /// Registers the User from the given request and returns the registered user.
    /// </summary>
    /// <param name="request">The UserRegisterRequest to register a user from.</param>
    /// <returns>The registered User to return.</returns>
    /// <exception cref="InvalidInputException">If the UserRegisterRequest is not valid.</exception>
    [HttpPost("users", Name = "RegisterUser")]
    public User RegisterUser([FromBody]UserRegisterRequest request) {
        // The ModelState is automatically populated by .NET during model binding and validation of the UserRegisterRequest.
        // This request comes from the HTTP request's body. After validating against the validation attributes (like [Required] etc.)
        // This ModelState will contain any validation errors that occurred. 
        if (!ModelState.IsValid) {
            // If the model state is invalid, it means the user entered an invalid value for one of the fields.
            // When this happens, we throw our custom InvalidInputException, which returns a 400 from our GlobalExceptionHandler.
            throw new InvalidInputException("User Registration Request is not valid.", ModelState);
        }
        return this.UserService.RegisterUser(request);
    }

}