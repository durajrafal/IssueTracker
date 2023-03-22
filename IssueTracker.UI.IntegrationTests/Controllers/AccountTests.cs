using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace IssueTracker.UI.IntegrationTests.Controllers
{
    public class AccountTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AccountTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }



        [Theory]
        [InlineData("LoginAsDeveloper")]
        [InlineData("LoginAsManager")]
        [InlineData("LoginAsAdmin")]
        public async void Login_AsDemoUser_DontReloadTheLoginPage(string action)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            var response = await client.GetAsync($"/Account/{action}");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.NotEqual("/Account/Login", response.Headers.Location.OriginalString);
        }


    }
}
