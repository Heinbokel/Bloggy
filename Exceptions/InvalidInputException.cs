using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bloggy.Exceptions {

    /// <summary>
    /// This exception is thrown when the requester provides invalid input in a request body.
    /// </summary>
    public class InvalidInputException : Exception
    {
        // This is the ModelState of our request body, which contains the validation errors that have occured.
        public ModelStateDictionary ModelState {get; set;}

        /// <summary>
        /// Constructor account for a custom message as well as the ModelStateDictionary containing the validation errors.
        /// </summary>
        /// <param name="message">The custom exception message.</param>
        /// <param name="modelState">The ModelStateDictionary containing the validation errors.</param>
        /// <returns>The InvalidInputException to return.</returns>
        public InvalidInputException(string message, ModelStateDictionary modelState): base(message) {
            this.ModelState = modelState;
        }
    }

}