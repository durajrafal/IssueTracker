using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment;
using IssueTracker.Infrastructure.Identity;

namespace IssueTracker.Application.IntegrationTests.Projects.Queries
{
    public class GetProjectDetailsForManagmentTests : TestsWithMediatorFixture
    {
        public GetProjectDetailsForManagmentTests() : base()
        {

        }

        [Fact]
        public async Task Handle_WhenProjectIdIsValid_ShouldReturnMembersAndAllOtherUsers()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenProjectIdIsValid_ShouldReturnMembersAndAllOtherUsers));

            //Act
            var query = new GetProjectDetailsForManagment { ProjectId = project.Id};
            var result = await Mediator.Send(query);

            //Assert
            var projectMembersCount = project.Members.Count;
            var allUsersCount = Database.Func<AuthDbContext, int>(ctx => ctx.Users.Count());
            var otherUsersCount = allUsersCount - projectMembersCount;
            Assert.Equal(project.Title, result.Title);
            Assert.Equal(projectMembersCount, result.Members.Count());
            Assert.Equal(otherUsersCount, result.OtherUsers.Count());
        }

        [Fact]
        public async Task Handle_WhenMemberIsMissingUser_ShouldReturnOnlyMembersWithExistingUsers()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenMemberIsMissingUser_ShouldReturnOnlyMembersWithExistingUsers),
                skipUser: true);

            //Act
            var query = new GetProjectDetailsForManagment { ProjectId = project.Id};
            var result = await Mediator.Send(query);

            //Assert
            var projectMembersCount = project.Members.Count-1;
            Assert.Equal(projectMembersCount, result.Members.Count());
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsInvalid_ThrowsNotFoundException()
        {
            //Arrange
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsInvalid_ThrowsNotFoundException));

            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            //Act
            var query = new GetProjectDetailsForManagment { ProjectId = 2};

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(() => Mediator.Send(query));
            Assert.True(Database.Func(x => x.Projects.Count() > 0));
        }

        [Fact]
        public async Task Handle_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException),
                false);

            //Act
            var query = new GetProjectDetailsForManagment() { ProjectId = project.Id };
            Func<Task> act = async () => await Mediator.Send(query);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
