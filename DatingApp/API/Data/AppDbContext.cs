using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // One to One relationship between AppUser and Member - Shared Primary Key
            builder.Entity<AppUser>()
                .HasOne(u => u.Member)
                .WithOne(m => m.User)
                .HasForeignKey<Member>(m => m.Id);

            // One to Many relationship between Member and Photo
            builder.Entity<Member>()
                .HasMany(m => m.Photos)
                .WithOne(p => p.Member)
                .HasForeignKey(p => p.MemberId);
        }

    }
}
