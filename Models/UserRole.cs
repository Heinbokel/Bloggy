namespace Bloggy.Models;

public class UserRole {

    public int Id { get; set;}
    public string Name { get; set;}
    public string Description { get; set;}

    // Navigation Properties. 
    // One UserRole can belong to many Users.
    public List<User> Users{ get; set;}

}