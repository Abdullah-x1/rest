using Microsoft.EntityFrameworkCore;
using DSAR.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DSAR.Data 
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserId)
                .IsUnique(); // 👈 This makes Email unique
        }
        public DbSet<User> User { get; set; }
        public DbSet<test> test { get; set; }

    }

}