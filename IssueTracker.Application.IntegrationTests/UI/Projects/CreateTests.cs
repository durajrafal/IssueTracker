using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Application.IntegrationTests.UI.Projects
{
    public class CreateTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public CreateTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_WhenUserInManagerRole_CreateProject()
        {
            //Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Manager")};
            var localFactory = _factory.MakeAuthenticatedWithClaims(claims);
            var client = localFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);
            var testing = new TestingHelpers(_factory);
            var title = nameof(Post_WhenUserInManagerRole_CreateProject);

            //Act
            var page = await client.GetAsync("/");
            var pageHtml = await page.Content.ReadAsStringAsync();

            var cookieToken = AntiForgeryHelpers.ExtractCookieToken(page.Headers);
            var formToken = AntiForgeryHelpers.ExtractFormToken(pageHtml, "test_csrf_field");

            var request = new HttpRequestMessage(HttpMethod.Post, "/Projects/Create");
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("title", title),
                new KeyValuePair<string,string>("test_csrf_field", formToken)
            });
            request.Headers.Add("Cookie", $"test_csrf_cookie={cookieToken}");
            var response = await client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("action=\"/Projects/Create\"", pageHtml);
            var userId = localFactory.Services.GetRequiredService<ICurrentUserService>().UserId;
            var addedProject = testing.FuncDatabase(ctx => ctx.Projects.Include(x => x.Members).First(x => x.Title == title));
            Assert.Contains(userId,addedProject.Members.Select(x => x.UserId));

        }

        [Fact]
        public async Task Post_WhenUserIsNotInRole_RedirectToLogin()
        {
            var client = _factory.MakeAuthenticated().CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            var page = await client.GetAsync("/");
            var pageHtml = await page.Content.ReadAsStringAsync();

            Assert.DoesNotContain("action=\"/Projects/Create\"", pageHtml);
        }
    }
}
