namespace Bloggy.Exceptions {

    /// <summary>
    /// This class holds the error details which will be returned to the requester when an exception occurs.
    /// </summary>
    public class ErrorDetails
    {
        public int StatusCode {get; set;}
        public string? Message {get; set;}
        public string? ExceptionMessage {get; set;}
    }

}