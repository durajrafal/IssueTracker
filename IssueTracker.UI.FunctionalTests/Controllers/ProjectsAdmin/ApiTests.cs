using IssueTracker.Application.Projects.Commands.UpdateProject;
using IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment;
using IssueTracker.Domain.Constants;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace IssueTracker.UI.FunctionalTests.Controllers.ProjectsAdmin
{
    public class ApiTests : UiTestsFixture
    {
        private Project _project;
        private string URI { get => $"api/project-management/{_project.Id}"; }
        public ApiTests() : base()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "Manager"),
                new Claim(AppClaimTypes.ProjectAccess, "1")
            };
            AuthenticateFactory(claims);
            _project = ProjectHelpers.CreateTestProject(nameof(ApiTests))
                .AddToDatabaseAsync(Database)
                .SeedDatabaseWithMembersUsersAsync(Database).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task GetDetails_WhenAuthorized_ShouldReturnJsonWithProjectManagementDto()
        {
            //Act
            var response = await Client.GetAsync(URI);
            var contentString = await response.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<ProjectManagmentDto>(contentString);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.Should().Be(new MediaTypeHeaderValue("application/json", "utf-8"));
            content.Title.Should().Be(_project.Title);
            content.Members.Should().HaveCount(_project.Members.Count);
        }

        //TODO - find a way to test put/delete endpoints with antiforgery
    }
}
