using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;

namespace IssueTracker.UI.IntegrationTests.Views
{
    public class AuthTests : UiBaseTest
    {
        public AuthTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task GetHomePage_WhenUserNotAuthenticated_ShouldRedirect()
        {
            var client = Factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/Identity/Account/Login", response.Headers.Location.OriginalString);

        }

        [Fact]
        public async Task GetHomePage_WhenUserAuthenticated_ShouldLoadPage()
        {
            AuthenticateFactory();
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact]
        public void SeedDatabase_Always_ShouldHaveAllTestUsers()
        {
            var users = Database.Func<AuthDbContext, List<ApplicationUser>>(ctx =>
            {
                return ctx.Users.ToList();
            });

            foreach (var user in users)
            {
                Assert.NotNull(user.Email);
                Assert.NotNull(user.FirstName);
                Assert.NotNull(user.LastName);
                Assert.True(user.EmailConfirmed);
            }
            Assert.Contains("dev@test.com", users.Select(x => x.UserName));
            Assert.Contains("manager@test.com", users.Select(x => x.UserName));
            Assert.Contains("admin@test.com", users.Select(x => x.UserName));
        }

    }
}