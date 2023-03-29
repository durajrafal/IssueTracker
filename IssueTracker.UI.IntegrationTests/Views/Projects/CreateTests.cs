using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.UI.IntegrationTests.Views.Projects
{
    public class CreateTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public CreateTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_WhenUserInManagerRole_ShouldShowFormToCreateNewProject()
        {
            //Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Manager")};
            var localFactory = _factory.MakeAuthenticatedWithClaims(claims);
            var client = localFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            //Act
            var page = await client.GetAsync("/");
            var pageHtml = await page.Content.ReadAsStringAsync();

            //Assert
            Assert.Equal(HttpStatusCode.OK, page.StatusCode);
            Assert.Contains("action=\"/Projects/Create\"", pageHtml);
        }

        [Fact]
        public async Task Get_WhenUserIsNotInRole_ShouldNotShowFormToCreateNewProject()
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

        [Fact]
        public async Task Post_WhenUserInManagerRole_ShouldAddProjectToDatabase()
        {
            //Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Manager") };
            var localFactory = _factory.MakeAuthenticatedWithClaims(claims);
            var client = localFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);
            var testing = new TestingHelpers(localFactory);
            var model = new { Title = "Test Project" };

            //Act
            var response = await client.SendFormAsync(HttpMethod.Post, "/", "/Projects/Create", model);

            //Assert
            var userId = localFactory.Services.GetRequiredService<ICurrentUserService>().UserId;
            var addedProject = testing.FuncDatabase(ctx => ctx.Projects.Include(x => x.Members).First(x => x.Title == model.Title));
            Assert.Contains(userId, addedProject.Members.Select(x => x.UserId));
        }
    }
}
