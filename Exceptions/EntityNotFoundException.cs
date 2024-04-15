namespace Bloggy.Exceptions;

/// <summary>
/// Custom exception that is thrown when attempting to retrieve a required entity but that 
/// entity is not found.
/// </summary>
public class EntityNotFoundException: Exception {
    public EntityNotFoundException(string message) : base(message) { 
        
    }
}