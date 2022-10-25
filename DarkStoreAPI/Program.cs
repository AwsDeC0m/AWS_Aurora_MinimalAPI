using DarStoreAPI.Db;
using DarStoreAPI.Db.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

app.MapGet("/v1/stores",
    async (AppDbContext db) => await db.Stores.ToListAsync())
    .Produces<List<Store>>(StatusCodes.Status200OK);

app.MapGet("/v1/stores/{id:guid}",
    async (AppDbContext db, Guid id) =>
    await db.Stores.FindAsync(id))
    .Produces<Store>(StatusCodes.Status200OK);

app.MapDelete("/v1/stores/{id:guid}",
    async (AppDbContext db, Guid id) =>
    {
        if (await db.Stores.FindAsync(id) is Store store)
        {
            db.Stores.Remove(store);
            await db.SaveChangesAsync();
            return Results.Ok(store);
        }
        return Results.NotFound();
    });

app.MapPost("/v1/stores",
    async (AppDbContext db, Store store) =>
    {
        await db.Stores.AddAsync(store);
        await db.SaveChangesAsync();
        return Results.Created("", null);
    });

app.Run();

void ConfigureServices(IServiceCollection service, ConfigurationManager configManager)
{
    service.AddDbContext<AppDbContext>(
        opts =>
        {
            opts.UseNpgsql(configManager.GetConnectionString("AppDb"));
        }, ServiceLifetime.Transient);
}

public partial class Program { }
