using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace IssueTracker.UI.IntegrationTests.Controllers
{
    public class AccountTests : BaseTestWithScope
    {
        public AccountTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Theory]
        [InlineData("LoginAsDeveloper")]
        [InlineData("LoginAsManager")]
        [InlineData("LoginAsAdmin")]
        public async void Login_AsDemoUser_DontReloadTheLoginPage(string action)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            var response = await client.GetAsync($"/Account/{action}");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.NotEqual("/Account/Login", response.Headers.Location.OriginalString);
        }


        [Fact]
        public async void Login_WithValidCredentials_IdentityCookieInHeaderIsPresent()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            var userName = "valid@test.com";
            var password = "Pass123";
            using (var userManager = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var user = new ApplicationUser(userName);
                await userManager.CreateAsync(user, password);
            }

            //Act
            var request = new HttpRequestMessage(HttpMethod.Post, "/Account/Login");
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("Email", userName),
                new KeyValuePair<string,string>("Password", password)
            });
            var response = await client.SendAsync(request);

            //Assert
            var cookies = response.Headers
                .Where(x => x.Key == "Set-Cookie")
                .SelectMany(x => x.Value.Select(v => v.Split("=").First()));
            Assert.Contains(".AspNetCore.Identity.Application", cookies);
        }

        [Fact]
        public async void Login_WithInvalidCredentials_IdentityCookieInHeaderIsNotPresent()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            var userName = "invalid@test.com";
            var password = "Pass123"
                ;
            //Act
            var request = new HttpRequestMessage(HttpMethod.Post, "/Account/Login");
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("Email", userName),
                new KeyValuePair<string,string>("Password", password)
            });
            var response = await client.SendAsync(request);

            //Assert
            var cookies = response.Headers
                .Where(x => x.Key == "Set-Cookie")
                .SelectMany(x => x.Value.Select(v => v.Split("=").First()));
            Assert.DoesNotContain(".AspNetCore.Identity.Application", cookies);
        }


    }
}
