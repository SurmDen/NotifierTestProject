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
            optionsBuilder.UseSqlite("");
        }
    }
}
