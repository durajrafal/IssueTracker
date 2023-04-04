using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Areas.Identity.Controllers;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;

namespace IssueTracker.UI.IntegrationTests.Controllers
{
    public class AccountTests : BaseTest
    {

        public AccountTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Theory]
        [InlineData("LoginAsDeveloper")]
        [InlineData("LoginAsManager")]
        [InlineData("LoginAsAdmin")]
        public async void Login_WhenLoggedInAsDemoUser_ShouldReturnIdentityCookie(string action)
        {
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            var response = await client.GetAsync($"/Identity/Account/{action}");

            var cookie = response.Headers
                .Where(x => x.Key == "Set-Cookie")
                .SelectMany(x => x.Value.Select(v => v.Split(";").First())).First();
            Assert.Equal(".AspNetCore.Identity.Application", cookie.Split("=").First());
            Assert.NotEqual("", cookie.Split("=").Last());
        }

        [Fact]
        public async void Logout_Always_ShouldEmptyIdentityCookie()
        {
            var client = Factory
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });

            var response = await client.GetAsync("/Identity/Account/Logout");

            var cookie = response.Headers
                .Where(x => x.Key == "Set-Cookie")
                .SelectMany(x => x.Value.Select(v => v.Split(";").First())).First();
            Assert.Equal(".AspNetCore.Identity.Application", cookie.Split("=").First());
            Assert.Equal(string.Empty, cookie.Split("=").Last());
        }





    }
}
