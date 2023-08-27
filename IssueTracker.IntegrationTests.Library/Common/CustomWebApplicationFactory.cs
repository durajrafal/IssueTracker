using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Infrastructure.Identity;
using Moq;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;

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
                    .AddSingleton<ICurrentUserService, TestCurrentUserService>();

                services.AddAntiforgery(setup =>
                {
                    setup.Cookie.Name = "AntiForgeryCookie";
                    setup.FormFieldName = "AntiForgeryField";
                });
            });

            builder.ConfigureTestServices(services =>
            {
                services.Mock<IEmailService>(mock =>
                {
                    mock.Setup(x =>
                        x.SendConfirmationEmailAsync(It.IsNotNull<string>(),
                        It.IsNotNull<string>(),
                        It.IsNotNull<string>()))
                    .Returns(Task.FromResult(true));
                    mock.Setup(x =>
                        x.SendConfirmationEmailAsync("fail@test.com",
                        It.IsNotNull<string>(),
                        It.IsNotNull<string>()))
                    .Returns(Task.FromResult(false));


                    mock.Setup(x =>
                    x.SendResetPasswordEmailAsync(It.IsNotNull<string>(),
                    It.IsNotNull<string>(),
                    It.IsNotNull<string>()))
                    .Returns(Task.FromResult(true));
                    mock.Setup(x =>
                    x.SendResetPasswordEmailAsync("fail@test.com",
                    It.IsNotNull<string>(),
                    It.IsNotNull<string>()))
                    .Returns(Task.FromResult(false));
                });

                var mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                    services.BuildServiceProvider().GetRequiredService<UserManager<ApplicationUser>>(),
                    Mock.Of<IHttpContextAccessor>(),
                    Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                    null, null, null, null);

                services.Remove<IUserService>()
                    .AddScoped<IUserService>(serviceProvider =>
                    new UserService(serviceProvider.GetRequiredService<UserManager<ApplicationUser>>(), mockSignInManager.Object));
            });

        }


    }
}
