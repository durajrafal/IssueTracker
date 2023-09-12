using IssueTracker.Domain.Constants;
using IssueTracker.UI.Controllers;
using IssueTracker.UI.Models.Projects;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IssueTracker.UI.FunctionalTests.Controllers
{
    public class ProjectsControllerTests : UiTestsFixture
    {
        private ProjectsController _controller;
        public ProjectsControllerTests() : base()
        {
            var claims = new List<Claim>()
            {
                new Claim(AppClaimTypes.ProjectAccess, "1")
            };
            AuthenticateFactory(claims);
            _controller = CreateControllerWithContext<ProjectsController>();
        }

        [Fact]
        public async Task GetDetails_WhenCurrentUserIsMember_ShouldReturnViewWithModel()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(GetDetails_WhenCurrentUserIsMember_ShouldReturnViewWithModel));

            //Act
            var response = await _controller.Details(project.Id, string.Empty, string.Empty) as ViewResult;
            var model = response?.Model as ProjectViewModel;

            //Assert
            model.Id.Should().Be(project.Id);
            model.Title.Should().Be(project.Title);
            model.Audit.Created.Should().BeCloseTo(project.Created, TimeSpan.FromSeconds(10));
            model.Audit.CreatedBy.Should().NotBeNull();
        }

        [Fact]
        public async Task GetDetails_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(GetDetails_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException),false);

            //Act
            Func<Task> act = async () => await _controller.Details(project.Id, string.Empty, string.Empty);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
