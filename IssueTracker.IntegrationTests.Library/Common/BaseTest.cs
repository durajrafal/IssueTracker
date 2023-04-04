using IssueTracker.IntegrationTests.Library.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IssueTracker.IntegrationTests.Library.Common
{
    public class BaseTest : IClassFixture<CustomWebApplicationFactory>
    {
        public WebApplicationFactory<Program> Factory { get; protected set; }
        public DatabaseHelpers Database { get; protected set; }
        public IServiceScopeFactory ScopeFactory { get => Factory.Services.GetRequiredService<IServiceScopeFactory>();}

        public BaseTest(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            Database = new DatabaseHelpers(ScopeFactory);
        }

        public BaseTest()
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
