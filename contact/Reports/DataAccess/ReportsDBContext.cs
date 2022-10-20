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
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<AllReports> AllReports { get; set; }
    }
}
