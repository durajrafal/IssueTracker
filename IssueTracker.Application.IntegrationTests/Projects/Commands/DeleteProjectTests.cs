using FluentValidation;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Projects.Commands.Delete;
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
            var project = new Project { Title = nameof(Handle_WhenTitleAndIdMatch_DeleteProjectFromDatabase) };
            await Database.ActionAsync(ctx => ctx.Projects.AddAsync(project));
            var command = new DeleteProjectCommand { ProjectId = project.Id, Title = project.Title };

            var addedProjectId = await Mediator.Send(command);

            var deletedProject = Database.Func(ctx => ctx.Projects.Find(project.Id));
            Assert.Null(deletedProject);
        }

        [Fact]
        public async Task Handle_WhenTitleAndIdNotMatch_ShouldThrowValidationException()
        {
            var project = new Project { Title = nameof(Handle_WhenTitleAndIdNotMatch_ShouldThrowValidationException) };
            await Database.ActionAsync(ctx => ctx.Projects.AddAsync(project));
            var command = new DeleteProjectCommand { ProjectId = project.Id, Title = "Misspelled Title" };

            await Assert.ThrowsAsync<ValidationException>(() => Mediator.Send(command));
        }
    }
}
