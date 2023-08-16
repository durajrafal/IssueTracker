﻿using IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment;
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
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsValid_ShouldReturnMembersAndAllOtherUsers));

            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });
            foreach (var member in project.Members)
            {
                var appUser = new ApplicationUser(member.UserId.Substring(0, 8), "Name", "Surname");
                appUser.Id = member.UserId;
                await Database.ActionAsync<AuthDbContext>(ctx => ctx.Users.AddAsync(appUser));
            }

            var query = new GetProjectDetailsForManagment { ProjectId = project.Id};
            var result = await Mediator.Send(query);

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
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenMemberIsMissingUser_ShouldReturnOnlyMembersWithExistingUsers));

            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });
            foreach (var member in project.Members.Skip(1))
            {
                var appUser = new ApplicationUser(member.UserId.Substring(0, 8), "Name", "Surname");
                appUser.Id = member.UserId;
                await Database.ActionAsync<AuthDbContext>(ctx => ctx.Users.AddAsync(appUser));
            }

            //Act
            var query = new GetProjectDetailsForManagment { ProjectId = project.Id};
            var result = await Mediator.Send(query);

            //Assert
            var projectMembersCount = project.Members.Count-1;
            Assert.Equal(projectMembersCount, result.Members.Count());
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsInvalid_ThrowsInvalidOperationException()
        {
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsInvalid_ThrowsInvalidOperationException));

            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            var query = new GetProjectDetailsForManagment { ProjectId = 2};

            await Assert.ThrowsAsync<InvalidOperationException>(() => Mediator.Send(query));
            Assert.True(Database.Func(x => x.Projects.Count() > 0));
        }
    }
}
