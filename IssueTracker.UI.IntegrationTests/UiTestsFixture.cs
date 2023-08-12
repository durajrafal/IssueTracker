using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace IssueTracker.UI.IntegrationTests
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
