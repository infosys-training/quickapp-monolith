using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuickApp.Core.Infrastructure;
using QuickApp.Core.Services.Account;

namespace QuickApp.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Remove ALL registrations related to ApplicationDbContext to avoid dual-provider conflicts
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            // Also remove any IDbContextOptionsConfiguration that applies UseSqlServer
            var configDescriptors = services
                .Where(d => d.ServiceType.IsGenericType &&
                            d.ServiceType.GetGenericTypeDefinition().FullName?.Contains("IDbContextOptionsConfiguration") == true)
                .ToList();
            foreach (var d in configDescriptors)
                services.Remove(d);

            // Build options manually to ensure only InMemory provider is registered
            var dbName = "QuickAppTestDb_" + Guid.NewGuid().ToString();
            services.AddSingleton<DbContextOptions<ApplicationDbContext>>(sp =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseInMemoryDatabase(dbName);
                optionsBuilder.UseOpenIddict();
                return optionsBuilder.Options;
            });

            services.AddScoped<ApplicationDbContext>(sp =>
            {
                var options = sp.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
                var userIdAccessor = sp.GetRequiredService<IUserIdAccessor>();
                return new ApplicationDbContext(options, userIdAccessor);
            });

            // Replace the database seeder with a no-op version for testing
            services.RemoveAll<IDatabaseSeeder>();
            services.AddTransient<IDatabaseSeeder, NoOpDatabaseSeeder>();
        });
    }
}

internal class NoOpDatabaseSeeder : IDatabaseSeeder
{
    public Task SeedAsync() => Task.CompletedTask;
}
