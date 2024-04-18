using Bloggy.Exceptions;
using Bloggy.Models;
using Bloggy.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggy.Services;

public class BlogService {

    private readonly BloggyDbContext BloggyDbContext;
    private readonly UserService UserService;

    public BlogService(BloggyDbContext context, UserService userService) {
        this.BloggyDbContext = context;
        this.UserService = userService;
    }

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