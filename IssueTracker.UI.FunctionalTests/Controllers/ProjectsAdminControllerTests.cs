using IssueTracker.Application.Projects.Commands.UpdateProject;
using IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment;
using IssueTracker.Domain.Constants;
using IssueTracker.Domain.Entities;
using IssueTracker.UI.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IssueTracker.UI.FunctionalTests.Controllers
{
    public class ProjectsAdminControllerTests : UiTestsFixture
    {
        private Project _project;
        private ProjectsAdminController _controller;

        public ProjectsAdminControllerTests() : base()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "Manager"),
                new Claim(AppClaimTypes.ProjectAccess, "1")
            };
            AuthenticateFactory(claims);

            _project = SetupTestProjectAsync(nameof(ProjectsAdminControllerTests)).GetAwaiter().GetResult();

            _controller = CreateControllerWithContext<ProjectsAdminController>();
        }

        [Fact]
        public async Task GetDetails_WhenAuthorized_ShouldReturnOkWithProjectManagementDto()
        {
            //Act
            var response = await _controller.GetDetails(_project.Id) as Ok<ProjectManagmentDto>;

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeOfType<ProjectManagmentDto>();
            response.Value.Title.Should().Be(_project.Title);
            response.Value.Members.Should().HaveCount(_project.Members.Count);
        }

        [Fact]
        public async Task GetDetails_WhenInvalidId_ShouldReturnNotFound()
        {
            //Act
            var response = await _controller.GetDetails(0) as NotFound;

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetDetails_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var notCurrentUserProject = await SetupTestProjectAsync(nameof(GetDetails_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException),
                false);

            //Act
            Func<Task> act = async () => await _controller.GetDetails(notCurrentUserProject.Id);

            //Assert
            act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task PutEdit_WhenDataIsValid_ShouldUpdateProjectInDatabase()
        {
            //Arrange
            var command = new UpdateProject()
            {
                Id = _project.Id,
                Title = "Updated Title",
                Members = _project.Members.Skip(1)
            };

            //Act
            var response = await _controller.Edit(_project.Id, command);

            //Assert
            var updatedProject = Database.Func(ctx => ctx.Projects.Include(x => x.Members).First(x => x.Id == _project.Id));
            updatedProject.Title.Should().Be(command.Title);
            updatedProject.Members.Should().HaveCountLessThan(_project.Members.Count);
        }

        [Fact]
        public async Task PutEdit_WhenDataIsInvalid_ShouldReturnBadRequestWithMessages()
        {
            //Arrange
            var command = new UpdateProject()
            {
                Id = _project.Id,
                Title = "",
                Members = _project.Members
            };

            //Act
            var response = await _controller.Edit(_project.Id, command) as BadRequest<string>;

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(400);
            response.Value.Should().NotBeEmpty();
        }

        [Fact]
        public async Task DeleteDelete_WhenIdIsValidAndTitleMatch_ShouldReturnOkAndDeleteProjectFromDatabase()
        {
            //Act
            var response = await _controller.Delete(_project.Id, _project.Title) as Ok;

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            Database.Func(ctx => ctx.Projects.FirstOrDefault(x => x.Id == _project.Id)).Should().BeNull();
        }

        [Fact]
        public async Task DeleteDelete_WhenIdIsValidButTitleDoNotMatch_ShouldReturnBadRequestAndDoNotDeleteProjectFromDatabase()
        {
            //Act
            var response = await _controller.Delete(_project.Id, "misspelled title") as BadRequest<string>;

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(400);
            response.Value.Should().NotBeEmpty();
            Database.Func(ctx => ctx.Projects.FirstOrDefault(x => x.Id == _project.Id)).Should().NotBeNull();

        }
    }
}
