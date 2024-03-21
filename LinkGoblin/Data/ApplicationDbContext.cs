using LinkGoblin.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkGoblin.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Collection> Collections { get; init; }
    public DbSet<Bookmark> Bookmarks { get; init; }
    public DbSet<Tag> Tags { get; init; }
}