using System.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using IssueTracker.Domain.Constants;

namespace IssueTracker.UI.FunctionalTests.Views.Projects
{
    public class CreateViewTests : UiTestsFixture
    {
        private const string FORM_ACTION = "action=\"/Project-Management/Create\"";

        public CreateViewTests() : base()
        {

        }

        [Fact]
        public async Task Get_WhenUserInManagerRole_ShouldShowFormToCreateNewProject()
        {
            //Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Manager") };
            AuthenticateFactory(claims);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            //Act
            var page = await Client.GetAsync("/Project-Management/Create");
            var pageHtml = await page.Content.ReadAsStringAsync();

            //Assert
            Assert.Equal(HttpStatusCode.OK, page.StatusCode);
            Assert.Contains(FORM_ACTION, pageHtml);
        }

        [Fact]
        public async Task Get_WhenUserIsNotInRole_ShouldNotShowFormToCreateNewProject()
        {
            //Arrange
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            //Act
            var page = await Client.GetAsync("/");
            var pageHtml = await page.Content.ReadAsStringAsync();

            //Assert
            Assert.DoesNotContain(FORM_ACTION, pageHtml);
        }

        [Fact]
        public async Task Post_WhenUserInManagerRole_ShouldAddProjectToDatabase()
        {
            //Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Manager") };
            AuthenticateFactory(claims);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);
            var model = new { Title = nameof(Post_WhenUserInManagerRole_ShouldAddProjectToDatabase) };

            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, "/Project-Management/Create", model);

            //Assert
            var userId = Factory.Services.GetRequiredService<ICurrentUserService>().UserId;
            var addedProject = Database.Func(ctx => ctx.Projects.Include(x => x.Members).First(x => x.Title == model.Title));
            Assert.Contains(userId, addedProject.Members.Select(x => x.UserId));
        }
    }
}
