using MartinKulev.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MartinKulev.Data
{
    public class MartinKulevDbContext : DbContext
    {
        public DbSet<BugReport> BugReports { get; set; }

        public DbSet<FeatureSuggestion> FeatureSuggestions { get; set; }

        public MartinKulevDbContext(DbContextOptions<MartinKulevDbContext> options)
            : base(options)
        {
        }
    }
}
