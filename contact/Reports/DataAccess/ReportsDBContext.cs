using Microsoft.EntityFrameworkCore;

namespace Reports.DataAccess
{
    public class ReportsDBContext : DbContext
    {
        public ReportsDBContext()
        {

        }

        public ReportsDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<AllReports> AllReports { get; set; }
    }
}
