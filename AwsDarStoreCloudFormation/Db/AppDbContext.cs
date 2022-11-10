using DarkStoreCommonLib.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace AwsDarStoreCloudFormation.Db;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

    public DbSet<Store> Stores { get; set; }
    public DbSet<StoreOptions>  StoreOptions { get; set; }

}