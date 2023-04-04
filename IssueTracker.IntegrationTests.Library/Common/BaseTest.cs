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
        public WebApplicationFactory<Program> Factory { get; private set; }
        public TestingHelpers Testing { get; private set; }
        public IServiceScopeFactory ScopeFactory { get => Factory.Services.GetRequiredService<IServiceScopeFactory>();}

        public BaseTest(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            Testing = new TestingHelpers(Factory);
        }

        public BaseTest()
        {
            Factory = new CustomWebApplicationFactory();
            Testing = new TestingHelpers(Factory);
        }

        public void AuthenticateFactory()
        {
            Factory = Factory.MakeAuthenticated();
            Testing = new TestingHelpers(Factory);
        }

        public void AuthenticateFactory(List<Claim> claims)
        {
            Factory = Factory.MakeAuthenticatedWithClaims(claims);
            Testing = new TestingHelpers(Factory);
        }

        public HttpClient SetupClient()
        {
            return Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
        }

        public TempDataDictionary SetupTempData()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            return tempData;
        }

        public T GetScopedService<T>() where T : notnull
        {
            return ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<T>();
        }
    }
}
