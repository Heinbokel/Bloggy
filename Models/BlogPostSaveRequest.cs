using System.ComponentModel.DataAnnotations;

namespace Bloggy.Models;

/// <summary>
/// Represents what is required from the end user to save a blog post.
/// Additional data such as User ID and Date Posted are determined from outside of this class.
/// </summary>
public class BlogPostSaveRequest {

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Title { get; set;}

    [Required]
    [MinLength(1)]
    [MaxLength(4096)]
    public string Content { get; set;}

}