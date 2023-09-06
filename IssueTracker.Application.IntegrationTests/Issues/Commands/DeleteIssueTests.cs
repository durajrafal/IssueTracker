using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Issues.Commands.DeleteIssue;

namespace IssueTracker.Application.IntegrationTests.Issues.Commands
{
    public class DeleteIssueTests : TestsWithMediatorFixture
    {
        public DeleteIssueTests() : base()
        {

        }

        [Fact]
        public async Task Handle_WhenIdIsValid_DeleteProjectFromDatabase()
        {
            //Arrange
            var project = await SetupTestProjectAsync();
            var issue = project.Issues.First();

            //Act
            var command = new DeleteIssue(issue.Id);
            await Mediator.Send(command);

            //Assert
            Database.Func(ctx => ctx.Issues.FirstOrDefault(x => x.Id == issue.Id)).Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenIdIsInvalid_ShouldThrowNotFoundException()
        {
           //Act
            var command = new DeleteIssue(0);
            var act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(addCurrentUser: false);
            var issue = project.Issues.First();

            //Act
            var command = new DeleteIssue(issue.Id); 
            var act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
