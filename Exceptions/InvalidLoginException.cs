namespace Bloggy.Exceptions;

/// <summary>
/// Custom exception that is thrown when a login attempt fails.
/// </summary>
public class InvalidLoginException: Exception {
    public InvalidLoginException(string message) : base(message) { 
        
    }
}