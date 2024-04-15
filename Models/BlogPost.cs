namespace Bloggy.Models;

/// <summary>
/// Represents a blog post, created by a given user.
/// </summary>
public class BlogPost {

    public int Id { get; set;}
    public string Title { get; set;}
    public string Content { get; set;}
    public DateOnly DatePosted { get; set;}

    // Navigation Properties.
    // A BlogPost can belong to only one User.
    public int UserId { get; set;}
    public User User { get; set;}

}