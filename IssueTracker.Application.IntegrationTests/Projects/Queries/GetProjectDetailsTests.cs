using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Domain.Entities;
using IssueTracker.Infrastructure.Identity;

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
            var project = await ProjectHelpers
                .CreateTestProject(nameof(Handle_WhenProjectIdIsValid_ShouldGetDetailsIncludingMembersAndIssuesWithMembers))
                .AddToDatabaseAsync(Database)
                .SeedDatabaseWithMembersUsersAsync(Database);

            var query = new GetProjectDetails { ProjectId = project.Id};
            var result = await Mediator.Send(query);

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
            var project = await ProjectHelpers
                .CreateTestProject(nameof(Handle_WhenMemberIsMissingUser_ShoulIncludeOnlyMembersWithExistingUsers))
                .AddToDatabaseAsync(Database)
                .SeedDatabaseWithMembersUsersAsync(Database,1);

            var query = new GetProjectDetails { ProjectId = project.Id};
            var result = await Mediator.Send(query);

            Assert.Equal(project.Members.Count-1, result.Members.Count());
            Assert.True(result.Members.All(x => x.User != null));
            Assert.True(result.Issues.All(x => x.Members.All(y => y.User != null)));
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsInvalid_ThrowsInvalidOperationException()
        {
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsInvalid_ThrowsInvalidOperationException));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            var query = new GetProjectDetails { ProjectId = 0 };

            await Assert.ThrowsAsync<InvalidOperationException>( () => Mediator.Send(query));
            Assert.True(Database.Func(x => x.Projects.Count() > 0));
        }


    }
}
