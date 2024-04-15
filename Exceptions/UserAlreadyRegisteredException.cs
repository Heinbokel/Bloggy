namespace Bloggy.Exceptions;

public class UserAlreadyRegisteredException: Exception {
    public UserAlreadyRegisteredException(string message) : base(message) { 
        
    }
}