namespace Bloggy.Models;

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