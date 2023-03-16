using FluentValidation;
using IssueTracker.Application.IntegrationTests.Common;
using IssueTracker.Application.Projects.Commands.CreateProject;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.IntegrationTests.Project.Commands
{
    public class CreateProjectTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly Testing _testing;

        public CreateProjectTests(CustomWebApplicationFactory factory)
        {
            _testing = new Testing(factory);
        }

        [Fact]
        public async Task Handle_WhenTitleNotEmptyAndUnique_ShouldAddToDatabase()
        {
            var command = new CreateProjectCommand { Title =  "Unique Test project"};
            var currentUser = await UserFixture.RunWithNoClaims();

            var addedProjectId = await _testing.SendAsync(command);

            var addedProject = _testing.FuncDatabase(ctx => ctx.Projects.First(x => x.Id == addedProjectId));
            Assert.Equal(command.Title, addedProject.Title);
        }

        [Fact]
        public async Task Handle_WhenTitleIsEmpty_ShouldThrowValidationException()
        {
            var command = new CreateProjectCommand();

            await Assert.ThrowsAsync<ValidationException>(() => _testing.SendAsync(command));
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

            await Assert.ThrowsAsync<ValidationException>(() => _testing.SendAsync(command));
        }

        [Fact]
        public async Task Handle_Always_ShouldAddCurrentUserAsMember()
        {
            var command = new CreateProjectCommand { Title = nameof(Handle_Always_ShouldAddCurrentUserAsMember) };
            var currentUserId = await UserFixture.RunWithNoClaims();

            var addedProjectId = await _testing.SendAsync(command);

            var addedProject = _testing.FuncDatabase(ctx => ctx.Projects.Include(x => x.Members).First(x => x.Id == addedProjectId));
            Assert.Equal(currentUserId, addedProject.Members.Single().UserId);
        }
    }
}
