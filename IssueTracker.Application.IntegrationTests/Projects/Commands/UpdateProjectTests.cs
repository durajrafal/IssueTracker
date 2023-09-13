using FluentValidation;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Commands.UpdateProject;
using IssueTracker.Domain.Constants;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
        public async Task Handle_WhenProjectIdIsValid_ShouldUpdateProjectInDatabase()
        {
            //Arrange
            var project = await SetupTestProjectAsync();

            //Act
            var command = new UpdateProject
            {
                Id = project.Id,
                Title = "Updated Title",
                Members = project.Members
            };
            await Mediator.Send(command);

            //Assert
            var updatedProject = Database.Func(ctx => ctx.Projects.Include(x => x.AuditEvents).First(x => x.Id == project.Id));
            updatedProject.Title.Should().Be(command.Title);
            updatedProject.Created.Should().Be(project.Created);
            updatedProject.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            updatedProject.LastModifiedById.Should().Be(GetCurrentUserId());
            updatedProject.AuditEvents.Should().NotBeNullOrEmpty();
            updatedProject.AuditEvents.Select(x => x.Timestamp).Should().AllSatisfy(x => x.Equals(updatedProject.LastModified));
            updatedProject.AuditEvents.Select(x => x.ModifiedById).Should().AllBe(GetCurrentUserId());
            var titleUpdateEvent = updatedProject.AuditEvents.First(x => x.PropertyName == "Title").DeserializeValuesPropertiesAsString();
            titleUpdateEvent.OldValue.Should().Be(project.Title);
            titleUpdateEvent.NewValue.Should().Be(command.Title);
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsValidAndTitleUnique_ShouldAddNewUser()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenProjectIdIsValidAndTitleUnique_ShouldAddNewUser));
            var projectMembersStartCount = project.Members.Count;   
            ICollection<Member> members = project.Members;
            members.Add(new Member { UserId = Guid.NewGuid().ToString() });

            //Act
            var command = new UpdateProject 
            { 
                Id = project.Id, 
                Title = project.Title,
                Members = members
            };
            await Mediator.Send(command);

            //Assert
            var udpatedProject = Database.Func(ctx => 
                ctx.Projects.Include(x => x.Members).First(x => x.Id == project.Id)
            );
            Assert.Equal(projectMembersStartCount + 1, udpatedProject.Members.Count);
        }
        
        [Fact]
        public async Task Handle_WhenProjectIdIsValidAndTitleUnique_ShouldDeleteUser()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenProjectIdIsValidAndTitleUnique_ShouldDeleteUser));
            var projectMembersStartCount = project.Members.Count;
            var projectUser = project.Members.First();
            project.Members.Remove(projectUser);
            ICollection<Member> members = project.Members;

            //Act
            var command = new UpdateProject 
            { 
                Id = project.Id, 
                Title = project.Title,
                Members = members
            };
            await Mediator.Send(command);

            //Assert
            var udpatedProject = Database.Func(ctx => 
                ctx.Projects.Include(x => x.Members).First(x => x.Id == project.Id)
            );
            Assert.Equal(projectMembersStartCount - 1, udpatedProject.Members.Count);
        }

        [Fact]
        public async Task Handle_WhenTitleIsNotUnique_ThrowsValidationException()
        {
            //Arrange
            var project = new Project { Title = nameof(Handle_WhenTitleIsNotUnique_ThrowsValidationException) };
            string takenTitle = "Already taken title";
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
                await ctx.Projects.AddAsync(new Project { Title = takenTitle });
            });

            //Act
            var command = new UpdateProject { Id = project.Id, Title = takenTitle };

            //Assert
            await Assert.ThrowsAsync<ValidationException>(() => Mediator.Send(command));
        }

        [Fact]
        public async Task Handle_WhenMembersAddedAndRemoved_ShouldAddAndRemoveProjectAccessClaims()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenMembersAddedAndRemoved_ShouldAddAndRemoveProjectAccessClaims));
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

        [Fact]
        public async Task Handle_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(Handle_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException),
                false);

            //Act
            var command = new UpdateProject()
            {
                Id = project.Id,
                Title = project.Title,
                Members = project.Members
            };
            Func<Task> act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
