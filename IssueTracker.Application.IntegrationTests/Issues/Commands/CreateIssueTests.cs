using FluentValidation;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Domain.Enums;
using IssueTracker.Application.Issues.Commands.CreateIssue;

namespace IssueTracker.Application.IntegrationTests.Issues.Commands
{
    public class CreateIssueTests : TestsWithMediatorFixture
    {
        public CreateIssueTests(): base()
        {

        }

        [Fact]
        public async Task Handle_WhenTitleNotEmptyAndUnique_ShouldAddToDatabase()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenTitleNotEmptyAndUnique_ShouldAddToDatabase));

            //Act
            var command = new CreateIssue { Title = nameof(Handle_WhenTitleNotEmptyAndUnique_ShouldAddToDatabase), ProjectId = project.Id};
            var addedIssueId = await Mediator.Send(command);

            //Assert
            var addedIssue = Database.Func(ctx => ctx.Issues.First(x => x.Id == addedIssueId));
            addedIssue.Title.Should().Be(command.Title);
        }

        [Fact]
        public async Task Handle_WhenTitleIsEmpty_ShouldThrowValidationException()
        {
            //Act
            var command = new CreateIssue();
            var act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_WhenTitleIsNotUniqueWithinTheProject_ShouldThrowValidationException()
        {
            //Arrange
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenTitleIsNotUniqueWithinTheProject_ShouldThrowValidationException),
                GetCurrentUserId());
            project.Issues.Add(new Issue { Title = "Not unique issue title" });
            await project.AddToDatabaseAsync(Database);

            //Act
            var command = new CreateIssue { Title = "Not unique issue title", ProjectId = project.Id };
            var act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_WhenIssueIsAddedToDatabase_ShouldBeConnectedToParentProject()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenIssueIsAddedToDatabase_ShouldBeConnectedToParentProject));

            //Act
            var command = new CreateIssue { Title = nameof(Handle_WhenIssueIsAddedToDatabase_ShouldBeConnectedToParentProject), ProjectId = project.Id };
            var addedIssueId = await Mediator.Send(command);

            //Assert
            var parentProject = Database.Func(ctx => ctx.Projects.Include(x => x.Issues).First(x => x.Id == project.Id));
            var issueInParentProject = parentProject.Issues.FirstOrDefault(x => x.Id == addedIssueId);
            issueInParentProject.Should().NotBeNull();
            issueInParentProject?.Title.Should().Be(command.Title);
        }

        [Fact]
        public async Task Handle_WhenIssueIsAddedToDatabase_ShouldHaveAllDataSaved()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenIssueIsAddedToDatabase_ShouldHaveAllDataSaved));
            var member = project.Members.First();

            //Act
            var command = new CreateIssue
            {
                Title = nameof(Handle_WhenIssueIsAddedToDatabase_ShouldHaveAllDataSaved),
                ProjectId = project.Id,
                Description = "My project description",
                Priority = PriorityLevel.High,
                Members = new List<Member> { new Member() { UserId= Guid.NewGuid().ToString() }, member }
            };
            var addedIssueId = await Mediator.Send(command);

            //Assert
            var addedIssue = Database.Func(ctx => ctx.Issues.Include(x => x.Members).First(x => x.Id == addedIssueId));
            addedIssue.Title.Should().Be(command.Title);
            addedIssue.Description.Should().Be(command.Description);
            addedIssue.Priority.Should().Be(command.Priority);
            addedIssue.Members.Should().HaveCount(2);
            addedIssue.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            addedIssue.CreatedBy.Should().Be(GetCurrentUserId());
            addedIssue.LastModified.Should().BeNull();
            addedIssue.LastModifiedById.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenIssueIsAddedToDatabase_ShouldHaveCorrectEnumsValuesByDefault()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenIssueIsAddedToDatabase_ShouldHaveCorrectEnumsValuesByDefault));

            //Act
            var command = new CreateIssue
            {
                Title = nameof(Handle_WhenIssueIsAddedToDatabase_ShouldHaveCorrectEnumsValuesByDefault),
                ProjectId = project.Id,
            };
            var addedIssueId = await Mediator.Send(command);

            //Assert
            var addedIssue = Database.Func(ctx => ctx.Issues.First(x => x.Id == addedIssueId));
            addedIssue.Priority.Should().Be(PriorityLevel.None);
            addedIssue.Status.Should().Be(WorkingStatus.Pending);
        }

        [Fact]
        public async Task Handle_WhenIssueIsAddedToProjectWithoutMembership_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenIssueIsAddedToProjectWithoutMembership_ShouldThrowUnauthorizedAccessException),
                false);

            //Act
            var command = new CreateIssue
            {
                Title = nameof(Handle_WhenIssueIsAddedToProjectWithoutMembership_ShouldThrowUnauthorizedAccessException),
                ProjectId = project.Id
            };
            var act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
