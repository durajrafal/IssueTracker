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
        private const string LOGIN_URI = "/Identity/Account/Login";

        public AccountTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Theory]
        [InlineData("LoginAsDeveloper")]
        [InlineData("LoginAsManager")]
        [InlineData("LoginAsAdmin")]
        public async void Login_WhenLoggedInAsDemoUser_ShouldHaveIdentityCookie(string action)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            var response = await client.GetAsync($"/Identity/Account/{action}");

            var cookies = response.Headers
                .Where(x => x.Key == "Set-Cookie")
                .SelectMany(x => x.Value.Select(v => v.Split("=").First()));
            Assert.Contains(".AspNetCore.Identity.Application", cookies);
        }


        

        


    }
}
