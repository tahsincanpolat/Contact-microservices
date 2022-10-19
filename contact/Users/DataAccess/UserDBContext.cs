using Microsoft.EntityFrameworkCore;
using Users.Models;

namespace Users.DataAccess
{
    public class UserDBContext : DbContext
    {
        public UserDBContext()
        {
           
        }

        public UserDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
