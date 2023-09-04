using FluentValidation;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Issues.Commands.UpdateIssue;
using IssueTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.IntegrationTests.Issues.Commands
{
    public class UpdateIssueTests : TestsWithMediatorFixture
    {
        public UpdateIssueTests() : base()
        {
        }

        [Fact]
        public async Task Handle_WhenTitleIsValidAndNotEmpty_ShouldUpdateIssueInDatabase()
        {
            //Arrange
            var project = await SetupTestProjectAsync();
            var issue = project.Issues.First();

            //Act
            var command = new UpdateIssue()
            {
                Id = issue.Id,
                Title = "Updated title",
                Description = "Updated description",
                Priority = PriorityLevel.Low,
                Status = WorkingStatus.Completed,
                ProjectId = project.Id
            };
            await Mediator.Send(command);

            //Assert
            var updatedIssue = await Database.Func(ctx => ctx.Issues.FirstAsync(x => x.Id == issue.Id));
            updatedIssue.Title.Should().Be(command.Title);
            updatedIssue.Description.Should().Be(command.Description);
            updatedIssue.Priority.Should().Be(command.Priority);
            updatedIssue.Status.Should().Be(command.Status);
        }

        [Fact]
        public async Task Handle_WhenTitleIsNotUniqueWithinTheProject_ShouldThrowValidationException()
        {
            //Arrange
            var project = await SetupTestProjectAsync();
            var issue = project.Issues.First();

            //Act
            var command = new UpdateIssue()
            {
                Id = issue.Id,
                Title = project.Issues.Last().Title,
                ProjectId = issue.ProjectId
            };
            var act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(addCurrentUser: false);
            var issue = project.Issues.First();

            //Act
            var command = new UpdateIssue()
            {
                Id = issue.Id,
                Title = "Updated title",
                Description = "Updated description",
                Priority = PriorityLevel.Low,
                Status = WorkingStatus.Completed,
                ProjectId = project.Id
            };
            var act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsInvalid_ThrowsNotFoundException()
        {
            //Arrange
            var project = await SetupTestProjectAsync();
            var issue = project.Issues.First();

            //Act
            var command = new UpdateIssue() { Id = 0, Title = "Some Random Title", ProjectId = project.Id };
            var act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
