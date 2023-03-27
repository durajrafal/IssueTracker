using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Infrastructure.Identity;

namespace IssueTracker.IntegrationTests.Library.Common
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                var dbName = Guid.NewGuid().ToString();
                services.Remove<DbContextOptions<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
                services.Remove<DbContextOptions<AuthDbContext>>();
                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
                services.Remove<ICurrentUserService>()
                    .AddSingleton<ICurrentUserService, TestUserService>();

                services.AddAntiforgery(setup =>
                {
                    setup.Cookie.Name = "test_csrf_cookie";
                    setup.FormFieldName = "test_csrf_field";
                });
            });

        }

        
    }
}
