using Consumer.Models;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
        
        public DbSet<Unit> Units { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetLiveStatus> AssetLiveStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AssetLiveStatus>(entiny =>
            {
                entiny.ToTable("AssetLiveStatus");
                
                entiny.HasKey(a => a.AssetId);

                entiny.HasOne(a => a.Asset)
                    .WithOne(a => a.AssetLiveStatus)
                    .HasForeignKey<AssetLiveStatus>(a => a.AssetId);
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Units");

                entity.HasKey(u => u.Id);
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.ToTable("Assets");

                entity.HasKey(a => a.Id);

                entity.HasOne(a => a.Unit)
                    .WithMany(u => u.Assets)
                    .HasForeignKey(a => a.UnitId);
            });
        }
}