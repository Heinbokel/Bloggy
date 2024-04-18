using Bloggy.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Bloggy.Repositories;

/// <summary>
/// DbContext for the Bloggy application.
/// </summary>
public class BloggyDbContext: DbContext {
    public IConfiguration Configuration {get; set;}

    public DbSet<User> Users { get; set;}
    public DbSet<UserRole> UserRoles { get; set;}
    public DbSet<BlogPost> BlogPosts { get; set;}

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="configuration">The IConfiguration to provide to this class.</param>
    public BloggyDbContext(IConfiguration configuration) {
        Configuration = configuration;
    }

    /// <summary>
    /// Logic used to configure the DbContext.
    /// </summary>
    /// <param name="optionsBuilder">The DbContextOptionsBuilder to use.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));

        optionsBuilder.EnableDetailedErrors();
    }

    /// <summary>
    /// Logic to run when creating models/migrations.
    /// </summary>
    /// <param name="builder">The ModelBuilder to use.</param>
    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<UserRole>().HasData(
            new UserRole {Id = 1, Name = "ADMIN", Description = "User Role designating elevated administrative priveleges."},
            new UserRole {Id = 2, Name = "USER", Description = "User Role designating regular priveleges."}
        );
    }

}