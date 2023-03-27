using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace IssueTracker.UI.IntegrationTests.Controllers
{
    public class AccountTests : BaseTestWithScope
    {
        private const string LoginUri = "/Identity/Account/Login";
        private const string RegisterUri = "/Identity/Account/Register";

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

            var response = await client.GetAsync($"/Identity/Account/{action}");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.NotEqual(LoginUri, response.Headers.Location.OriginalString);
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
                var user = new ApplicationUser(userName, "John", "Smith");
                await userManager.CreateAsync(user, password);
            }

            //Act
            var request = new HttpRequestMessage(HttpMethod.Post, LoginUri);
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
            var password = "Pass123";

            //Act
            var request = new HttpRequestMessage(HttpMethod.Post, LoginUri);
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

        [Fact]
        public async void Register_WithRequiredFields_AddNewUserToDatabase()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            var user = new RegisterViewModel
            {
                Email = "mock@test.com",
                Password = "Pass123",
                ConfirmPassword = "Pass123",
                FirstName = "Registered",
                LastName = "User"
            };

            //Act
            var request = new HttpRequestMessage(HttpMethod.Post, RegisterUri);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("Email", user.Email),
                new KeyValuePair<string,string>("Password", user.Password),
                new KeyValuePair<string,string>("ConfirmPassword", user.ConfirmPassword),
                new KeyValuePair<string,string>("FirstName", user.FirstName),
                new KeyValuePair<string,string>("LastName", user.LastName),
            });
            var response = await client.SendAsync(request);

            //Assert
            var registeredUser = _testing.FuncDatabase<AuthDbContext, ApplicationUser>(ctx => 
            ctx.Users.Where(x => x.Email == user.Email).First());
            Assert.NotNull(registeredUser);
            Assert.Equal(user.Email, registeredUser.UserName);
            Assert.Equal(user.Email, registeredUser.Email);
            Assert.Equal(user.FirstName, registeredUser.FirstName);
            Assert.Equal(user.LastName, registeredUser.LastName);
        }


    }
}
