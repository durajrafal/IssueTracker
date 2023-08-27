using IssueTracker.UI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace IssueTracker.UI.FunctionalTests
{
    public class UiTestsFixture : TestsFixture
    {
        public HttpClient Client { get; private set; }
        public UiTestsFixture() : base()
        {
            Client = SetupClient();
        }

        public void AuthenticateFactory()
        {
            Factory = Factory.MakeAuthenticated();
            UpdateFactoryDependentProperties();
        }

        public void AuthenticateFactory(List<Claim> claims)
        {
            Factory = Factory.MakeAuthenticatedWithClaims(claims);
            UpdateFactoryDependentProperties();
        }

        public T CreateControllerWithContext<T>() where T: ControllerWithMediatR, new()
        {
            var httpContext = new DefaultHttpContext()
            {
                RequestServices = ScopeFactory.CreateScope().ServiceProvider
            };
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            return new T()
            {
                ControllerContext = controllerContext
            };
        }

        private void UpdateFactoryDependentProperties()
        {
            Database = new DatabaseHelpers(ScopeFactory);
            Client = SetupClient();
        }

        private HttpClient SetupClient()
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

    }

}
