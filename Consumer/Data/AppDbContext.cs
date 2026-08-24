using Microsoft.EntityFrameworkCore;
using Producer.Models;

namespace Consumer.Data;

public class AppDbContext: DbContext
{
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<AssetLiveReport> AssetLiveReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder<AssetLiveReport>(entiny =>
            {
                entiny.
            });
        }
}