using IssueTracker.Application.Common.Exceptions;
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
            var updatedTitle = "UpdatedTitle";
            var project = await SetupTestProjectAsync(nameof(Handle_WhenProjectIdIsValid_ShouldGetDetailsIncludingMembersAndIssuesWithMembers));
            await Database.ActionAsync(ctx =>
            {
                var entity = ctx.Projects.First(x => x.Id == project.Id);
                entity.Title = updatedTitle;
            });

            //Act
            var query = new GetProjectDetails { ProjectId = project.Id};
            var result = await Mediator.Send(query);

            //Assert
            result.Title.Should().Be(updatedTitle);
            Assert.Equal(project.Members.Count, result.Members.Count());
            Assert.True(result.Members.All(x => x.User != null));
            Assert.Equal(project.Issues.Count, result.Issues.Count);
            Assert.True(result.Issues.All(x => x.Title != String.Empty));
            Assert.True(result.Issues.All(x => x.Members.Count > 0));
            result.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            result.CreatedByUser.Should().NotBeNull();
            result.CreatedByUser.UserId.Should().Be(GetCurrentUserId());            
            result.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10))
                .And.BeAfter(result.Created);
            result.LastModifiedByUser.Should().NotBeNull();
            result.LastModifiedByUser!.UserId.Should().Be(GetCurrentUserId());
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
        public async Task Handle_WhenProjectIdIsInvalid_ThrowsNotFoundException()
        {
            //Arrange
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsInvalid_ThrowsNotFoundException), GetCurrentUserId());
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            //Act
            var query = new GetProjectDetails { ProjectId = 0 };

            //Assert
            await Assert.ThrowsAsync<NotFoundException>( () => Mediator.Send(query));
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
