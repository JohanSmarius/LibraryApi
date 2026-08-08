using LibraryApi.Controllers.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swashbuckle for the STARTING solution. The minimal API version of this
// project swaps this block for builder.Services.AddOpenApi() (built into
// .NET 10) instead.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Library API (Controllers)",
        Version = "v1",
        Description = "Starting solution: controller-based Web API with Authors and Books, ready to be migrated to minimal APIs."
    });
});

var connectionString = builder.Configuration.GetConnectionString("LibraryDb")
    ?? "Data Source=library.db";

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Apply any pending EF Core migrations and seed known data on startup.
// This is the "reset button" behaviour: delete library.db (or run
// scripts/reset-db.*) and the next run always comes back to the same
// known state - no manual re-seeding live on stage.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.Migrate();
    SeedData.EnsureSeeded(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
