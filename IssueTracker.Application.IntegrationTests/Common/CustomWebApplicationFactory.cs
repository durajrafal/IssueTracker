using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Infrastructure.Identity;

namespace IssueTracker.Application.IntegrationTests.Common
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                services.Remove<DbContextOptions<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IssueTracker");
                });
                services.Remove<DbContextOptions<AuthDbContext>>();
                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IssueTracker");
                });
                services.Remove<ICurrentUserService>()
                    .AddSingleton<ICurrentUserService, TestUserService>();

                services.AddAntiforgery(setup =>
                {
                    setup.Cookie.Name = "test_csrf_cookie";
                    setup.FormFieldName = "test_csrf_field";
                });
            });

            builder.UseEnvironment("Development");
        }

        
    }
}
