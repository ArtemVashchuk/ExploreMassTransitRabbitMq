using Microsoft.EntityFrameworkCore;
using Newsletter.Api.Entities;

namespace Newsletter.Api.Database;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(builder =>
            builder.OwnsOne(a => a.Tags, tagsBuilder => tagsBuilder.ToJson()));
    }

    public DbSet<Article> Articles { get; set; }
}
