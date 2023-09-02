using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            result.Members.Should().HaveCount(issue.Members.Count());
            result.Members.Should().AllSatisfy(x => x.User.Should().NotBeNull());
            result.Project.Should().NotBeNull();
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
