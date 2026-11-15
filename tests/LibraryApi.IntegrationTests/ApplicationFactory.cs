using LibraryApi.Controllers.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LibraryApi.IntegrationTests;

public class ApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Filename=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the default DbContext registration from Program.cs
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LibraryDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Create the schema and apply migrations before Program.cs runs
            _connection.Open();
            using (var db = new LibraryDbContext(new DbContextOptionsBuilder<LibraryDbContext>()
                .UseSqlite(_connection)
                .Options))
            {
                db.Database.Migrate();
            }

            // Register DbContext with the shared connection
            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });
    }

    public async Task InitializeAsync()
    {
        // Reset the database before each test
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

        // Delete all data but keep the schema
        // Books must be deleted first due to cascade relationship
        db.Books.ExecuteDelete();
        db.Authors.ExecuteDelete();
    }

    public new async Task DisposeAsync()
    {
        _connection.Dispose();
        await base.DisposeAsync();
    }
}
