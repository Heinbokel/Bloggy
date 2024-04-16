using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace Bloggy.Exceptions {

    /// <summary>
    /// This class acts as an interceptor which will catch all exceptions before they reach the requester. 
    /// We will handle each exception type as needed to provide a well constructed response to the requester.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// Catches an exception and handles it by returning an ErrorDetails object to the consumer.
        /// </summary>
        /// <param name="httpContext">Holds information about our current HTTP request.</param>
        /// <param name="exception">The exception that has occurred that needs to be handled.</param>
        /// <param name="cancellationToken">In simple terms, a cancellation token is like a way to politely ask your task to stop
        ///  working if something else comes up. It's a handy tool for making your programs more responsive and flexible.</param>
        /// <returns>True if the exception was successfully handled.</returns>
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Create our ErrorDetails to be filled out depending on what type of exception has occurred.
            ErrorDetails errorDetails = new ErrorDetails();

            if (exception is EntityNotFoundException) {
                errorDetails.StatusCode = (int) HttpStatusCode.NotFound;
                errorDetails.Message = "Entity was not found.";
                errorDetails.ExceptionMessage = exception.Message;
            } else if (exception is UserAlreadyRegisteredException) {
                errorDetails.StatusCode = (int) HttpStatusCode.BadRequest;
                errorDetails.Message = "Username or Email is not available.";
                errorDetails.ExceptionMessage = exception.Message;
            } else if (exception is InvalidInputException) {
                errorDetails.StatusCode = (int) HttpStatusCode.BadRequest;
                errorDetails.Message = GenerateInvalidInputMessage((InvalidInputException)exception);
                errorDetails.ExceptionMessage = exception.Message;
            } else if (exception is InvalidLoginException) {
                errorDetails.StatusCode = (int) HttpStatusCode.Forbidden;
                errorDetails.Message = "Resource is forbidden.";
                errorDetails.ExceptionMessage = exception.Message;
            } else {
                errorDetails.StatusCode = (int) HttpStatusCode.InternalServerError;
                errorDetails.Message = "Something went wrong.";
                errorDetails.ExceptionMessage = exception.Message;
            }

            // Set the status code of the HTTP Response and the Content Type of the HTTP Response.
            httpContext.Response.StatusCode = errorDetails.StatusCode;
            httpContext.Response.ContentType = "application/json";
            
            // Write our error details as JSON for the response body.
            await httpContext.Response.WriteAsJsonAsync(errorDetails, cancellationToken);

            // Return true as we have successfully handled our exception.
            return true;
        }

        /// <summary>
        /// Generates a descriptive error message that contains all validation errors.
        /// </summary>
        /// <param name="exception">The InvalidInputException to generate the error message for.</param>
        /// <returns>The error message to return.</returns>
        private string GenerateInvalidInputMessage(InvalidInputException exception)
        {
            string message = exception.Message;

            // We are going to add the entire collection of validation errors to our single error message to return.
            // For each value in our model state dictionary, we are going to put all of their error message's in to a list.
            List<string> errors = exception.ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

            // Join the error message list back into a single string and append to the original message.
            message += string.Join(", ", errors); 

            return message;
        }
    }

}