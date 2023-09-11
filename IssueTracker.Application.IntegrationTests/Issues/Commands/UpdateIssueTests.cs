using FluentValidation;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Issues.Commands.UpdateIssue;
using IssueTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
                Status = WorkingStatus.InProgress,
                ProjectId = project.Id
            };
            await Mediator.Send(command);

            //Assert
            var updatedIssue = await Database.Func(ctx => 
                ctx.Issues.Include(x => x.AuditEvents).FirstAsync(x => x.Id == issue.Id));
            updatedIssue.Title.Should().Be(command.Title);
            updatedIssue.Description.Should().Be(command.Description);
            updatedIssue.Priority.Should().Be(command.Priority);
            updatedIssue.Status.Should().Be(command.Status);
            updatedIssue.Created.Should().Be(issue.Created);
            updatedIssue.LastModified.Should().NotBeNull()
                .And.BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            updatedIssue.LastModifiedBy.Should().Be(GetCurrentUserId());
            updatedIssue.AuditEvents.Should().NotBeNullOrEmpty();
            updatedIssue.AuditEvents.Select(x => x.Timestamp).Should().AllSatisfy(x => x.Equals(updatedIssue.LastModified));
            updatedIssue.AuditEvents.Select(x => x.ModifiedBy).Should().AllBe(GetCurrentUserId());
            var titleUpdateEvent = updatedIssue.AuditEvents.First(x => x.PropertyName == "Title");
            titleUpdateEvent.OldValue.Should().Be(JsonSerializer.Serialize(issue.Title));
            titleUpdateEvent.NewValue.Should().Be(JsonSerializer.Serialize(command.Title));
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
