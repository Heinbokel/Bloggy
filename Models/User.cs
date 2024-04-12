namespace Bloggy.Models;

public class User {

    public int Id { get; set;}
    public string UserName { get; set;}
    public string Email { get; set;}
    public string FirstName { get; set;}
    public string LastName { get; set;}
    public DateOnly DateOfBirth { get; set;}

    // Navigation Properties.
    // A User can have only one UserRole.
    public int UserRoleId { get; set;}
    public UserRole UserRole { get; set;}

    // A User can have many BlogPosts.
    public List<BlogPost> BlogPosts { get; set;}


}