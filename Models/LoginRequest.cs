using System.ComponentModel.DataAnnotations;

namespace Bloggy.Models;

/// <summary>
/// Represents the login details necessary to login a user.
/// </summary>
public class LoginRequest {
    
    [Required]
    [EmailAddress]
    public string Email { get; set;}

    [Required]
    [MinLength(8)]
    public string Password { get; set;}

}