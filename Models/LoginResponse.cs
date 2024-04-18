namespace Bloggy.Models;

/// <summary>
/// Represents the login response returned after logging in a user.
/// The Token contains the JWT represented as an encoded string.
/// </summary>
public class LoginResponse {

    public string Token {get; set;}

}