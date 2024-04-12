using Bloggy.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Bloggy.Repositories;

public class BloggyDbContext: DbContext {
    public IConfiguration Configuration {get; set;}

    public DbSet<User> Users { get; set;}
    public DbSet<UserRole> UserRoles { get; set;}
    public DbSet<BlogPost> BlogPosts { get; set;}

    public BloggyDbContext(IConfiguration configuration) {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));

        optionsBuilder.EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<UserRole>().HasData(
            new UserRole {Id = 1, Name = "ADMIN", Description = "User Role designating elevated administrative priveleges."},
            new UserRole {Id = 2, Name = "USER", Description = "User Role designating regular priveleges."}
        );
    }

}