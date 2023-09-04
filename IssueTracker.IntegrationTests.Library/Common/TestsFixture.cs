using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using IssueTracker.IntegrationTests.Library.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IssueTracker.IntegrationTests.Library.Common
{
    public abstract class TestsFixture : IClassFixture<CustomWebApplicationFactory>
    {
        public WebApplicationFactory<Program> Factory { get; protected set; }
        public DatabaseHelpers Database { get; protected set; }
        public IServiceScopeFactory ScopeFactory { get => Factory.Services.GetRequiredService<IServiceScopeFactory>();}

        public TestsFixture()
        {
            Factory = new CustomWebApplicationFactory();
            Database = new DatabaseHelpers(ScopeFactory);
        }

        public T GetScopedService<T>() where T : notnull
        {
            return ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<T>();
        }

        public string GetCurrentUserId()
        {
            return Factory.Services.GetRequiredService<ICurrentUserService>().UserId;
        }

        public async Task<Project> SetupTestProjectAsync(string title = "Project Title", bool addCurrentUser = true, bool skipUser = false)
        {
            var currentUserId = addCurrentUser ? GetCurrentUserId() : "";
            var numberOfUsersToSkip = skipUser ? 1 : 0;
            return await ProjectHelpers.CreateTestProject(title, currentUserId)
                .AddToDatabaseAsync(Database)
                .SeedDatabaseWithMembersUsersAsync(Database, numberOfUsersToSkip);
        }
    }
}
