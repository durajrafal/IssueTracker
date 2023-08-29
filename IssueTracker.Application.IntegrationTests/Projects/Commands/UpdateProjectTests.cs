using FluentAssertions;
using FluentValidation;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Commands.UpdateProject;
using IssueTracker.Domain.Constants;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Application.IntegrationTests.Projects.Commands
{
    public class UpdateProjectTests : TestsWithMediatorFixture
    {
        public UpdateProjectTests() : base()
        {

        }

        [Fact]
        public async Task Handle_WhenProjectIdIsInvalid_ThrowsNotFoundException()
        {
            var command = new UpdateProject { Id = 0 , Title = nameof(Handle_WhenProjectIdIsInvalid_ThrowsNotFoundException) };

            await Assert.ThrowsAsync<NotFoundException>(() => Mediator.Send(command));
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsValidAndTitleUnique_ShouldAddNewUser()
        {
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsValidAndTitleUnique_ShouldAddNewUser));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });
            var projectMembersStartCount = project.Members.Count;   
            ICollection<Member> members = project.Members;
            members.Add(new Member { UserId = Guid.NewGuid().ToString() });

            var command = new UpdateProject 
            { 
                Id = project.Id, 
                Title = project.Title,
                Members = members 
            };
            await Mediator.Send(command);

            var udpatedProject = Database.Func(ctx => 
                ctx.Projects.Include(x => x.Members).First(x => x.Id == project.Id)
            );
            Assert.Equal(projectMembersStartCount + 1, udpatedProject.Members.Count);
        }
        
        [Fact]
        public async Task Handle_WhenProjectIdIsValidAndTitleUnique_ShouldDeleteUser()
        {
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsValidAndTitleUnique_ShouldDeleteUser));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });
            var projectMembersStartCount = project.Members.Count;
            var projectUser = project.Members.First();
            project.Members.Remove(projectUser);
            ICollection<Member> members = project.Members;

            var command = new UpdateProject 
            { 
                Id = project.Id, 
                Title = project.Title,
                Members = members
            };
            await Mediator.Send(command);

            var udpatedProject = Database.Func(ctx => 
                ctx.Projects.Include(x => x.Members).First(x => x.Id == project.Id)
            );
            Assert.Equal(projectMembersStartCount - 1, udpatedProject.Members.Count);
        }

        [Fact]
        public async Task Handle_WhenTitleIsNotUnique_ThrowsValidationException()
        {
            var project = new Project { Title = nameof(Handle_WhenTitleIsNotUnique_ThrowsValidationException) };
            string takenTitle = "Already taken title";
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
                await ctx.Projects.AddAsync(new Project { Title = takenTitle });
            });

            var command = new UpdateProject { Id = project.Id, Title = takenTitle };

            await Assert.ThrowsAsync<ValidationException>(() => Mediator.Send(command));
        }

        [Fact]
        public async Task Handle_WhenMembersAddedAndRemoved_ShouldAddAndRemoveProjectAccessClaims()
        {
            //Arrange
            var project = await ProjectHelpers
                .CreateTestProject(nameof(Handle_WhenMembersAddedAndRemoved_ShouldAddAndRemoveProjectAccessClaims))
                .AddToDatabaseAsync(Database)
                .SeedDatabaseWithMembersUsersAsync(Database);
            var addedUserId = Guid.NewGuid().ToString();
            await IdentityHelpers.AddIdentityUserFromUserIdAsync(addedUserId, Database);
            ICollection<Member> members = project.Members.Skip(1).ToList();
            members.Add(new Member() { UserId = addedUserId });
            var userService = GetScopedService<IUserService>();

            //Act
            var command = new UpdateProject()
            {
                Id = project.Id,
                Title = project.Title,
                Members = members
            };
            await Mediator.Send(command);

            //Assert
            var removedUserClaims = await userService.GetUserClaimsAsync(project.Members.First().UserId);
            var addedUserClaims = await userService.GetUserClaimsAsync(addedUserId);
            removedUserClaims.Should().NotContain(x => x.Type == AppClaimTypes.ProjectAccess && x.Value == project.Id.ToString());
            addedUserClaims.Should().Contain(x => x.Type == AppClaimTypes.ProjectAccess && x.Value == project.Id.ToString());
        }
    }
}
