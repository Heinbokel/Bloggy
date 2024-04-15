namespace Bloggy.Exceptions;

/// <summary>
/// Custom exception that is thrown when an attempt to register a user that already exists is made.
/// </summary>
public class UserAlreadyRegisteredException: Exception {
    public UserAlreadyRegisteredException(string message) : base(message) { 
        
    }
}