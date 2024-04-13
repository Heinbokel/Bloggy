using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace Bloggy.Models;

[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class User {

    public int Id { get; set;}

    public string UserName { get; set;}
    public string Email { get; set;}
    public string FirstName { get; set;}
    public string LastName { get; set;}
    public DateOnly DateOfBirth { get; set;}

    [JsonIgnore]
    public string Password { get; set;}

    [JsonIgnore]
    public byte[] Salt {get; set;}

    // Navigation Properties.
    // A User can have only one UserRole.
    public int UserRoleId { get; set;}
    
    [JsonIgnore]
    public UserRole UserRole { get; set;}

    // A User can have many BlogPosts.
    [JsonIgnore]
    public List<BlogPost> BlogPosts { get; set;}


}