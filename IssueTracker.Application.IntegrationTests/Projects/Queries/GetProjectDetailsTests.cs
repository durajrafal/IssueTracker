using IssueTracker.Application.Projects.Queries.GetProjectDetails;

namespace IssueTracker.Application.IntegrationTests.Projects.Queries
{
    public class GetProjectDetailsTests : TestsWithMediatorFixture
    {
        public GetProjectDetailsTests() : base()
        {

        }

        [Fact]
        public async Task Handle_WhenProjectIdIsValid_ShouldGetDetailsIncludingMembersAndIssuesWithMembers()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenProjectIdIsValid_ShouldGetDetailsIncludingMembersAndIssuesWithMembers));

            //Act
            var query = new GetProjectDetails { ProjectId = project.Id};
            var result = await Mediator.Send(query);

            //Assert
            Assert.Equal(project.Members.Count, result.Members.Count());
            Assert.True(result.Members.All(x => x.User != null));
            Assert.Equal(project.Issues.Count, result.Issues.Count);
            Assert.True(result.Issues.All(x => x.Title != String.Empty));
            Assert.True(result.Issues.All(x => x.Members.Count > 0));
            Assert.True(result.Issues.All(x => x.Priority == Domain.Enums.PriorityLevel.None));
        }
        
        [Fact]
        public async Task Handle_WhenMemberIsMissingUser_ShoulIncludeOnlyMembersWithExistingUsers()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenMemberIsMissingUser_ShoulIncludeOnlyMembersWithExistingUsers), 
                skipUser: true);

            //Act
            var query = new GetProjectDetails { ProjectId = project.Id};
            var result = await Mediator.Send(query);

            //Assert
            Assert.Equal(project.Members.Count-1, result.Members.Count());
            Assert.True(result.Members.All(x => x.User != null));
            Assert.True(result.Issues.All(x => x.Members.All(y => y.User != null)));
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsInvalid_ThrowsInvalidOperationException()
        {
            //Arrange
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsInvalid_ThrowsInvalidOperationException), GetCurrentUserId());
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            //Act
            var query = new GetProjectDetails { ProjectId = 0 };

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>( () => Mediator.Send(query));
            Assert.True(Database.Func(x => x.Projects.Count() > 0));
        }

        [Fact]
        public async Task Handle_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException), false);

            //Act
            var query = new GetProjectDetails { ProjectId = project.Id };
            Func<Task> act = async () => await Mediator.Send(query);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
