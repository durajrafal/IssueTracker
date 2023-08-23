using FluentAssertions;
using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Commands.DeleteProject;
using IssueTracker.Domain.Constants;
using IssueTracker.Domain.Entities;

namespace IssueTracker.Application.IntegrationTests.Projects.Commands
{
    public class DeleteProjectTests : TestsWithMediatorFixture
    {
        public DeleteProjectTests() : base()
        {
            
        }

        [Fact]
        public async Task Handle_WhenTitleAndIdMatch_DeleteProjectFromDatabase()
        {
            //Arrange
            var project = new Project { Title = nameof(Handle_WhenTitleAndIdMatch_DeleteProjectFromDatabase) };
            await Database.ActionAsync(ctx => ctx.Projects.AddAsync(project));

            //Act
            var command = new DeleteProject { ProjectId = project.Id, Title = project.Title };
            await Mediator.Send(command);

            //Assert
            var deletedProject = Database.Func(ctx => ctx.Projects.Find(project.Id));
            Assert.Null(deletedProject);
        }

        [Fact]
        public async Task Handle_WhenTitleAndIdNotMatch_ShouldThrowValidationException()
        {
            //Arrange
            var project = new Project { Title = nameof(Handle_WhenTitleAndIdNotMatch_ShouldThrowValidationException) };
            await Database.ActionAsync(ctx => ctx.Projects.AddAsync(project));

            //Act
            var command = new DeleteProject { ProjectId = project.Id, Title = "Misspelled Title" };
            
            //Assert
            await Assert.ThrowsAsync<ValidationException>(() => Mediator.Send(command));
        }

        [Fact]
        public async Task Handle_WhenProjectDeletedFromDatabase_ShouldRemoveProjectAccessClaimFromAllMembers()
        {
            //Arrange
            var userService = GetScopedService<IUserService>();
            var project = await ProjectHelpers
                .CreateTestProject(nameof(Handle_WhenProjectDeletedFromDatabase_ShouldRemoveProjectAccessClaimFromAllMembers))
                .AddToDatabaseAsync(Database)
                .SeedDatabaseWithMembersUsersAsync(Database);

            var members = project.Members.ToList();
            members.ForEach(async x => await userService.AddProjectAccessClaimToUserAsync(x.UserId, project.Id));

            //Act
            var command = new DeleteProject { ProjectId = project.Id, Title = project.Title };
            await Mediator.Send(command);

            //Assert
            foreach (var member in members)
            {
                var claims = await userService.GetUserClaimsAsync(member.UserId);
                claims.FirstOrDefault(x => x.Type == AppClaimTypes.ProjectAccess)?.Value.Should().BeNull();
            }
        }
    }
}
