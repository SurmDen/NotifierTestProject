using Microsoft.EntityFrameworkCore;
using NotifierTestProject.Entities;

namespace NotifierTestProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            Database.EnsureCreated();
        }

        private readonly IConfiguration _configuration;

        public DbSet<User> Users { get; set; }

        public DbSet<Notice> Notices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_configuration.GetConnectionString("SQLiteConnection") ?? "notice_app.db");
        }

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
