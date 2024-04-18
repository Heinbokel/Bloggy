using System.Security.Claims;
using Bloggy.Exceptions;
using Bloggy.Models;
using Bloggy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Controllers;

/// <summary>
/// Controller used for routing requests regarding the BlogPost entity.
/// </summary>
[ApiController]
public class BlogPostsController : ControllerBase
{
    private readonly BlogService BlogService;

    public BlogPostsController(BlogService blogService)
    {
        this.BlogService = blogService;
    }

    [Authorize]
    [HttpPost("blog-posts", Name = "CreateBlogPost")]
    public BlogPost CreateBlogPost([FromBody] BlogPostSaveRequest request)
    {
        // The ModelState is automatically populated by .NET during model binding and validation of the BlogPostSaveRequest.
        // This request comes from the HTTP request's body. After validating against the validation attributes (like [Required] etc.)
        // This ModelState will contain any validation errors that occurred. 
        if (!ModelState.IsValid)
        {
            // If the model state is invalid, it means the user entered an invalid value for one of the fields.
            // When this happens, we throw our custom InvalidInputException, which returns a 400 from our GlobalExceptionHandler.
            throw new InvalidInputException("Login Request is not valid.", ModelState);
        }

        /* Attempt to retrieve the user's identity using the HttpContext.
        * When an HTTP request is received by an ASP.NET Core application, it flows through a series of middleware components
        * in the request processing pipeline. One of these middleware components is responsible for populating
        * the HttpContext with information about the current request, including details such as headers, query parameters, and user identity.
        * The HttpContext is passed to the controller method by the MVC framework as part of the request handling process.
        * ASP.NET Core automatically binds the relevant HTTP context to the controller method's parameter,
        * allowing you to access request-related information within the method.
        */
        int userId = JwtService.GetUserIdFromHttpContext(HttpContext);

        return BlogService.CreateBlogPost(request, userId);
    }

}