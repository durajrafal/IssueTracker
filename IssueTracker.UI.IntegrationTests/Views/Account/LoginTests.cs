using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.UI.IntegrationTests.Views.Account
{
    public class LoginTests : BaseTestWithScope
    {
        private const string LoginUri = "/Identity/Account/Login";
        public LoginTests(CustomWebApplicationFactory factory)
            :base(factory)
        {
        }

        [Fact]
        public async void LoginWithCredentials_WhenAreValid_RedirectToHomePage()
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
            var page = await client.GetAsync(LoginUri);
            var request = await CreateLoginFormRequestAsync(page, userName, password);
            var response = await client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/", response.Headers.Location.OriginalString);
        }

        [Fact]
        public async void LoginWithCredentials_WhenAreInvalid_DoNoRedirectToHome()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            var userName = "invalid@test.com";
            var password = "Pass123";

            //Act
            var page = await client.GetAsync(LoginUri);
            var request = await CreateLoginFormRequestAsync(page, userName, password);
            var response = await client.SendAsync(request);

            //Assert
            Assert.NotEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Null(response.Headers.Location);
        }

        private async Task<HttpRequestMessage> CreateLoginFormRequestAsync(HttpResponseMessage page, string userName, string password)
        {
            var pageHtml = await page.Content.ReadAsStringAsync();

            var cookieToken = AntiForgeryHelpers.ExtractCookieToken(page.Headers);
            var formToken = AntiForgeryHelpers.ExtractFormToken(pageHtml, "test_csrf_field");

            var output = new HttpRequestMessage(HttpMethod.Post, LoginUri);
            output.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("Email", userName),
                new KeyValuePair<string,string>("Password", password),
                new KeyValuePair<string,string>("test_csrf_field", formToken)
            });
            output.Headers.Add("Cookie", $"test_csrf_cookie={cookieToken}");

            return output;
        }
    }
}
