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

    [HttpPost("users", Name = "RegisterUser")]
    public User RegisterUser(UserRegisterRequest request) {
        return this.UserService.RegisterUser(request);
    }

}