using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.IntegrationTests.Common;
using IssueTracker.Application.IntegrationTests.Helpers;
using IssueTracker.Application.Projects.Commands.CreateProject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Application.IntegrationTests.Project.Commands
{
    public class CreateProjectTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly TestingHelpers _testing;
        private readonly CustomWebApplicationFactory _factory;

        public CreateProjectTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _testing = new TestingHelpers(_factory);
        }

        [Fact]
        public async Task Handle_WhenTitleNotEmptyAndUnique_ShouldAddToDatabase()
        {
            var command = new CreateProjectCommand { Title =  "Unique Test project"};

            var addedProjectId = await _testing.MediatorSendAsync(command);

            var addedProject = _testing.FuncDatabase(ctx => ctx.Projects.First(x => x.Id == addedProjectId));
            Assert.Equal(command.Title, addedProject.Title);
        }

        [Fact]
        public async Task Handle_WhenTitleIsEmpty_ShouldThrowValidationException()
        {
            var command = new CreateProjectCommand();

            await Assert.ThrowsAsync<ValidationException>(() => _testing.MediatorSendAsync(command));
        }

        [Fact]
        public async Task Handle_WhenTitleIsNotUnique_ShouldThrowValidationException()
        {
            _testing.ActionDatabase(async ctx =>
            {
                await ctx.Projects.AddAsync(new Domain.Entities.Project { Title = "Not unique test project" });
                await ctx.SaveChangesAsync();
            });

            var command = new CreateProjectCommand { Title = "Not unique test project" };

            await Assert.ThrowsAsync<ValidationException>(() => _testing.MediatorSendAsync(command));
        }

        [Fact]
        public async Task Handle_Always_ShouldAddCurrentUserAsMember()
        {
            var command = new CreateProjectCommand { Title = nameof(Handle_Always_ShouldAddCurrentUserAsMember) };

            var addedProjectId = await _testing.MediatorSendAsync(command);

            var userId = _factory.Services.GetRequiredService<ICurrentUserService>().UserId;
            var addedProject = _testing.FuncDatabase(ctx => ctx.Projects.Include(x => x.Members).First(x => x.Id == addedProjectId));
            Assert.Equal(userId, addedProject.Members.Single().UserId);
        }
    }
}
