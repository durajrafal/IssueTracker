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
    }
}
