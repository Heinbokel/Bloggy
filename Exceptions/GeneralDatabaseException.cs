namespace Bloggy.Exceptions;

public class GeneralDatabaseException: Exception {
    public GeneralDatabaseException(string message, Exception exception) : base(message, exception) { 
        
    }
}