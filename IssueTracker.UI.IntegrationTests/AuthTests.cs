﻿using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;

namespace IssueTracker.UI.IntegrationTests
{
    public class AuthTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly TestingHelpers _testing;

        public AuthTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _testing = new TestingHelpers(_factory);
        }

        [Fact]
        public async Task GetHomePage_WhenUserNotAuthenticated_Redirect()
        {
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/Account/Login",response.Headers.Location.OriginalString);

        }

        [Fact]
        public async Task GetHomePage_WhenUserAuthenticated_LoadPage()
        {
            var client = _factory.MakeAuthenticated().CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact]
        public void SeedDatabase_Always_HaveAllTestUsers()
        { 
            var users = _testing.FuncDatabase<AuthDbContext, List<ApplicationUser>>(ctx =>
            {
                return ctx.Users.ToList();
            });

            Assert.Contains("dev@test.com", users.Select(x => x.UserName));
            Assert.Contains("manager@test.com", users.Select(x => x.UserName));
            Assert.Contains("admin@test.com", users.Select(x => x.UserName));
        }

    }
}