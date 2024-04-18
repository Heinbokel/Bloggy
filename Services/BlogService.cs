using Bloggy.Exceptions;
using Bloggy.Models;
using Bloggy.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggy.Services;

/// <summary>
/// Business Logic for working with BlogPost entities.
/// </summary>
public class BlogService {

    private readonly BloggyDbContext BloggyDbContext;
    private readonly UserService UserService;

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="context">The BloggyDbContext to provide to this class.</param>
    /// <param name="userService">The UserService to provide to this class.</param>
    public BlogService(BloggyDbContext context, UserService userService) {
        this.BloggyDbContext = context;
        this.UserService = userService;
    }

    /// <summary>
    /// Saves a Blog Post to the database.
    /// </summary>
    /// <param name="request">The BlogPostSaveRequest to use.</param>
    /// <param name="userId">The ID of the signed on user.</param>
    /// <returns>The saved/created BlogPost to return.</returns>
    public BlogPost CreateBlogPost(BlogPostSaveRequest request, int userId) {
        // Retrieve User from User ID which was taken from the JWT.
        User? userForBlogPost = this.UserService.RetrieveUserById(userId);

        // If the user is not found (null is returned), throw EntityNotFoundException explaining such.
        // Generally this should never occur because the user ID comes from the JWT, which is
        // the signed on user making this request. But you never know.
        if (userForBlogPost == null) {
            throw new EntityNotFoundException($"User with ID {userId} was not found.");
        }

        // Create new BlogPost object that we will save.
        BlogPost blogPostToCreate = new BlogPost {
            Title = request.Title,
            Content = request.Content,
            User = userForBlogPost,
            UserId = userForBlogPost.Id,
            DatePosted = DateOnly.FromDateTime(DateTime.Today)
        };

        // Attempt to save to the database, catching any DbUpdateException that occurs.
        try {
            this.BloggyDbContext.BlogPosts.Add(blogPostToCreate);
            this.BloggyDbContext.SaveChanges();
            return blogPostToCreate;
        } catch (DbUpdateException exception) {
            throw new GeneralDatabaseException("An error occurred when attempting to save a blog post.", exception);
        }
    }

}