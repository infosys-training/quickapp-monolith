using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuickApp.Core.Infrastructure;
using QuickApp.Core.Services.Account;

namespace QuickApp.Tests;

public class OidcConfigurationTests
{
    [Fact]
    public void App_Startup_Fails_Without_OIDC_Certificates_In_Production()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Production");
                    builder.ConfigureServices(services =>
                    {
                        // Remove ALL registrations related to ApplicationDbContext
                        services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                        services.RemoveAll<ApplicationDbContext>();

                        var configDescriptors = services
                            .Where(d => d.ServiceType.IsGenericType &&
                                        d.ServiceType.GetGenericTypeDefinition().FullName?.Contains("IDbContextOptionsConfiguration") == true)
                            .ToList();
                        foreach (var d in configDescriptors)
                            services.Remove(d);

                        var dbName = "QuickAppOidcTestDb_" + Guid.NewGuid().ToString();
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

                        services.RemoveAll<IDatabaseSeeder>();
                        services.AddTransient<IDatabaseSeeder, NoOpDatabaseSeeder>();
                    });
                });

            // Force the app to start, which will trigger the OIDC configuration check
            _ = factory.CreateClient();
        });

        Assert.Contains("OIDC certificates must be configured for production", exception.Message);
    }
}
