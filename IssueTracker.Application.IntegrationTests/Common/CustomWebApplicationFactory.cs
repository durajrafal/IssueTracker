using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssueTracker.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Moq;
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
                services.AddSingleton<UserFixture>();

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
                    .AddScoped(serviceProvider => Mock.Of<ICurrentUserService>(s => s.UserId == UserFixture.GetCurrentUserId()));
            });

            builder.UseEnvironment("Development");
        }

        
    }
}
