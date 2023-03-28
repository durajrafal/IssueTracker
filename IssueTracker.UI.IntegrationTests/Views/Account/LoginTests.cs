using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace IssueTracker.UI.IntegrationTests.Views.Account
{
    public class LoginTests : BaseTestWithScope
    {
        private const string LOGIN_URI = "/Identity/Account/Login";
        public LoginTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async void LoginWithCredentials_WhenAreValid_HasIdentityCookieAndRedirectsToHomePage()
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
                var user = new ApplicationUser(userName, "John", "Smith");
                await userManager.CreateAsync(user, password);
            }
            var login = new LoginViewModel
            {
                Email = userName,
                Password = password,
                IsRememberChecked = false
            };

            //Act
            var response = client.SendFormAsync(HttpMethod.Post,LOGIN_URI,login).Result.HttpResponseMessage;

            //Assert
            var cookies = response.Headers
                .Where(x => x.Key == "Set-Cookie")
                .SelectMany(x => x.Value.Select(v => v.Split("=").First()));
            Assert.Contains(".AspNetCore.Identity.Application", cookies);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/", response.Headers.Location.OriginalString);
        }

        [Fact]
        public async void LoginWithCredentials_WhenAreInvalid_DoesntHaveIdentityCookieNorRedirectToHome()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            var userName = "invalid@test.com";
            var password = "Pass123";
            var login = new LoginViewModel
            {
                Email = userName,
                Password = password,
                IsRememberChecked = false
            };

            //Act
            var response = client.SendFormAsync(HttpMethod.Post, LOGIN_URI, login).Result.HttpResponseMessage;

            //Assert
            var cookies = response.Headers
                .Where(x => x.Key == "Set-Cookie")
                .SelectMany(x => x.Value.Select(v => v.Split("=").First()));
            Assert.DoesNotContain(".AspNetCore.Identity.Application", cookies);
            Assert.NotEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Null(response.Headers.Location);
        }
    }
}
