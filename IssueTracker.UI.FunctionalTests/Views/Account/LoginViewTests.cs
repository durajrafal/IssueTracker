using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace IssueTracker.UI.FunctionalTests.Views.Account
{
    public class LoginViewTests : UiTestsFixture
    {
        private const string LOGIN_URI = "/Identity/Account/Login";
        public LoginViewTests() : base()
        {

        }

        [Fact]
        public async void LoginWithCredentials_WhenCredentialsAreValid_ShouldHaveIdentityCookieAndRedirectsToHomePage()
        {
            //Arrange
            var userName = "valid@test.com";
            var password = "Pass123";
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
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
            var response = await Client.SendFormAsync(HttpMethod.Post,LOGIN_URI,login);

            //Assert
            var cookies = response.Headers
                .Where(x => x.Key == "Set-Cookie")
                .SelectMany(x => x.Value.Select(v => v.Split("=").First()));
            Assert.Contains(".AspNetCore.Identity.Application", cookies);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/", response.Headers.Location.OriginalString);
        }

        [Fact]
        public async void LoginWithCredentials_WhenCredentialsAreInvalid_ShouldNotHaveIdentityCookieNorRedirectToHome()
        {
            //Arrange
            var userName = "invalid@test.com";
            var password = "Pass123";
            var login = new LoginViewModel
            {
                Email = userName,
                Password = password,
                IsRememberChecked = false
            };

            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, LOGIN_URI, login);

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
