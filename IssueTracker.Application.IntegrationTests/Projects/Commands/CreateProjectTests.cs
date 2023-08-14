using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Commands.CreateProject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Application.IntegrationTests.Projects.Commands
{
    public class CreateProjectTests : TestsWithMediatorFixture
    {
        public CreateProjectTests() : base()
        {

        }

        [Fact]
        public async Task Handle_WhenTitleNotEmptyAndUnique_ShouldAddToDatabase()
        {
            var command = new CreateProject { Title =  "Unique Test project"};

            var addedProjectId = await Mediator.Send(command);

            var addedProject = Database.Func(ctx => ctx.Projects.First(x => x.Id == addedProjectId));
            Assert.Equal(command.Title, addedProject.Title);
        }

        [Fact]
        public async Task Handle_WhenTitleIsEmpty_ShouldThrowValidationException()
        {
            var command = new CreateProject();

            await Assert.ThrowsAsync<ValidationException>(() => Mediator.Send(command));
        }

        [Fact]
        public async Task Handle_WhenTitleIsNotUnique_ShouldThrowValidationException()
        {
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(new Domain.Entities.Project { Title = "Not unique test project" });
            });

            var command = new CreateProject { Title = "Not unique test project" };

            await Assert.ThrowsAsync<ValidationException>(() => Mediator.Send(command));
        }

        [Theory]
        [InlineData(':')]
        [InlineData('/')]
        [InlineData('?')]
        [InlineData('#')]
        [InlineData('[')]
        [InlineData(']')]
        [InlineData('@')]
        [InlineData('!')]
        [InlineData('$')]
        [InlineData('&')]
        [InlineData('\'')]
        [InlineData('(')]
        [InlineData(')')]
        [InlineData('*')]
        [InlineData('+')]
        [InlineData(',')]
        [InlineData(';')]
        [InlineData('=')]
        public async Task Handle_WhenTitleContainsForbiddenCharacter_ShouldThrowValidationException(char forbiddenChar)
        {
            var command = new CreateProject { Title = "Forbidden title" + forbiddenChar };

            await Assert.ThrowsAsync<ValidationException>(() => Mediator.Send(command));
        }

        [Fact]
        public async Task Handle_Always_ShouldAddCurrentUserAsMember()
        {
            var command = new CreateProject { Title = nameof(Handle_Always_ShouldAddCurrentUserAsMember) };

            var addedProjectId = await Mediator.Send(command);

            var userId = Factory.Services.GetRequiredService<ICurrentUserService>().UserId;
            var addedProject = Database.Func(ctx => ctx.Projects.Include(x => x.Members).First(x => x.Id == addedProjectId));
            Assert.Equal(userId, addedProject.Members.Single().UserId);
        }

        [Fact]
        public async Task Handle_Always_ShouldAddAlreadyCreatedMember()
        {
            var command = new CreateProject { Title = nameof(Handle_Always_ShouldAddAlreadyCreatedMember) };
            var command2 = new CreateProject { Title = nameof(Handle_Always_ShouldAddAlreadyCreatedMember)+'2' };

            await Mediator.Send(command);
            await Mediator.Send(command2);

            var userId = Factory.Services.GetRequiredService<ICurrentUserService>().UserId;

            Assert.Equal(1, Database.Func(x => x.Members.Where(m => m.UserId == userId).Count()));
        }
    }
}
