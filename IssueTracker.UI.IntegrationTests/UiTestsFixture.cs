using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace IssueTracker.UI.IntegrationTests
{
    public class UiTestsFixture : TestsFixture
    {
        public UiTestsFixture() : base()
        {

        }

        public void AuthenticateFactory()
        {
            Factory = Factory.MakeAuthenticated();
            Database = new DatabaseHelpers(ScopeFactory);
        }

        public void AuthenticateFactory(List<Claim> claims)
        {
            Factory = Factory.MakeAuthenticatedWithClaims(claims);
            Database = new DatabaseHelpers(ScopeFactory);
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

    }

}
