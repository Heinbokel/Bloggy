namespace Bloggy.Exceptions;

/// <summary>
/// Custom exception that is thrown when any unforeseen failures occur when calling a database.
/// </summary>
public class GeneralDatabaseException: Exception {
    public GeneralDatabaseException(string message, Exception exception) : base(message, exception) { 
        
    }
}