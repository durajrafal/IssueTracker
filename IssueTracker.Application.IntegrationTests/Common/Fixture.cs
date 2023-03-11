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

namespace IssueTracker.Application.IntegrationTests.Common
{
    public class Fixture : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<AppDbContext>));
                services.Remove(dbContextDescriptor);
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IssueTracker");
                });
            });

            builder.UseEnvironment("Development");
        }
    }
}
