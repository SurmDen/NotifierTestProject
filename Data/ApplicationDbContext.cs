using Microsoft.EntityFrameworkCore;
using NotifierTestProject.Entities;

namespace NotifierTestProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Notice> Notices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(userBuilder =>
            {
                userBuilder.HasIndex(x => x.Id);
                userBuilder.ToTable("users");
                userBuilder
                .HasMany(x => x.Notices)
                .WithMany(y => y.Users);
            });

            modelBuilder.Entity<Notice>(noticeBuilder =>
            {
                noticeBuilder.HasIndex(x => x.Id);
                noticeBuilder.ToTable("notices");
            });
        }
    }
}
