namespace HireMe.Data
{
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HireMe.Entities.Models;

    public class BaseDbContext : IdentityDbContext<User>
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) { }
    }

    public class FeaturesDbContext : DbContext
    {
        public FeaturesDbContext(DbContextOptions<FeaturesDbContext> options) : base(options) { }

        public DbSet<Contestant> Contestant { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Promotion> Promotion { get; set; }
        public DbSet<Resume> Resume { get; set; }
        public DbSet<Notification> Notification { get; set; }
    }

}
