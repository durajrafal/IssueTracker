using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Issues.Queries.GetIssueDetails;

namespace IssueTracker.Application.IntegrationTests.Issues.Queries
{
    public class GetIssueDetailsTests : TestsWithMediatorFixture
    {
        public GetIssueDetailsTests() : base()
        {
        }

        [Fact]
        public async Task Handle_WhenIdIsValid_ShouldReturnIssueDetailsIncludingMembersWithUsersAndProject()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenIdIsValid_ShouldReturnIssueDetailsIncludingMembersWithUsersAndProject));
            var issue = project.Issues.First();

            //Act
            var query = new GetIssueDetails(issue.Id);
            var result = await Mediator.Send(query);

            //Assert
            result.Title.Should().Be(issue.Title);
            result.Description.Should().Be(issue.Description);
            result.Priority.Should().Be(issue.Priority);
            result.Status.Should().Be(issue.Status);
            result.Members.Should().HaveCount(issue.Members.Count())
                .And.AllSatisfy(x => x.User.Should().NotBeNull());
            result.Project.Should().NotBeNull();
            result.Project.Members.Should().HaveCount(project.Members.Count)
                .And.AllSatisfy(x => x.User.Should().NotBeNull());
            result.Audit.Created.Should().BeCloseTo(project.Created, TimeSpan.FromSeconds(10));
            result.Audit.CreatedByUser.Should().NotBeNull();
            result.Audit.LastModified.Should().BeNull();
            result.Audit.LastModifiedBy.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenIssueWasUpdated_ShouldReturnLastModifiedAndAuditEvents()
        {
            //Arrange
            var updatedTitle = "UpdatedTitle";
            var project = await SetupTestProjectAsync(nameof(Handle_WhenIssueWasUpdated_ShouldReturnLastModifiedAndAuditEvents));
            var issue = project.Issues.First();
            await Database.ActionAsync(ctx =>
            {
                var entity = ctx.Issues.First(x => x.Id == issue.Id);
                entity.Title = updatedTitle;
            });

            //Act
            var query = new GetIssueDetails(issue.Id);
            var result = await Mediator.Send(query);

            //Assert
            result.Audit.LastModified.Should().BeCloseTo(project.Created, TimeSpan.FromSeconds(10))
                .And.BeAfter(result.Audit.Created);
            result.Audit.LastModifiedBy.Should().NotBeNull();
            result.Audit.LastModifiedBy!.UserId.Should().Be(GetCurrentUserId());
            result.Audit.AuditEvents.Items.Should().NotBeNullOrEmpty();
            var titleUpdateEvent = result.Audit.AuditEvents.Items.First(x => x.PropertyName == "Title").DeserializeValuesProperties();
            titleUpdateEvent.OldValue.Should().Be(issue.Title);
            titleUpdateEvent.NewValue.Should().Be(updatedTitle);
            titleUpdateEvent.ModifiedBy.Should().NotBeNull();
            titleUpdateEvent.ModifiedBy.UserId.Should().Be(GetCurrentUserId());
        }

        [Fact]
        public async Task Handle_WhenIdIsInvalid_ShouldThrowNotFoundException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenIdIsInvalid_ShouldThrowNotFoundException));

            //Act
            var query = new GetIssueDetails(0);
            var act = async () => await Mediator.Send(query);

            //Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenIssueFromProjectWithoutMembership_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenIssueFromProjectWithoutMembership_ShouldThrowUnauthorizedAccessException),
                false);

            //Act
            var query = new GetIssueDetails(project.Id);
            var act = async () => await Mediator.Send(query);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
