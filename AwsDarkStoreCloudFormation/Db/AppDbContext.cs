using DarkStoreCommonLib.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace AwsDarkStoreCloudFormation.Db;

public class AppDbContext : DbContext
{
    public AppDbContext() : base()
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        optionsBuilder.UseNpgsql("Host=xxxxxxxxxx.rds.amazonaws.com;Port=5432;Username=postgres;Password=postgres;Database=db_dark_store;");

        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Store> Stores { get; set; }
    public DbSet<StoreOptions> StoreOptions { get; set; }
}




