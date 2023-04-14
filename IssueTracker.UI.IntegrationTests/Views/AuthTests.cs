using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Areas.Identity.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace IssueTracker.UI.IntegrationTests.Views
{
    public class AuthTests : UiBaseTest
    {
        public AuthTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Theory]
        [ClassData(typeof(AuthorizeTestData))]
        public async Task GetEndpoint_WhenAuthorized_ShouldReturnEndpoint(string uri, List<Claim> claims, bool expectError = false)
        {
            AuthenticateFactory(claims);

            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            var response = await client.GetAsync(uri);
            if (expectError)
            {
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }
            else
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
                
        }

        [Theory]
        [ClassData(typeof(AuthorizeTestData))]
        public async Task GetEndpoint_WhenNotAuthorized_ShouldNotReturnEndpoint(string uri, List<Claim> claims, bool expectError = false)
        {
            bool hasClaim = claims.Count > 0;
            if (hasClaim)
            {
                AuthenticateFactory();
            }
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            var response = await client.GetAsync(uri);

            if (hasClaim)
                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            else
            {
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("http://localhost/Identity/Account/Login", response.Headers.Location.OriginalString);
            }
        }

        [Fact]
        public void SeedDatabase_Always_ShouldHaveAllTestUsers()
        {
            var users = Database.Func<AuthDbContext, List<ApplicationUser>>(ctx =>
            {
                return ctx.Users.ToList();
            });

            foreach (var user in users)
            {
                Assert.NotNull(user.Email);
                Assert.NotNull(user.FirstName);
                Assert.NotNull(user.LastName);
                Assert.True(user.EmailConfirmed);
            }
            Assert.Contains("dev@test.com", users.Select(x => x.UserName));
            Assert.Contains("manager@test.com", users.Select(x => x.UserName));
            Assert.Contains("admin@test.com", users.Select(x => x.UserName));
        }

    }

    public class AuthorizeTestData : IEnumerable<object[]>
    {
        private const string ACCOUNT_CONTROLLER = "Identity/Account/";
        private const string ADMIN_CONTROLLER = "Identity/Admin/";
        private const string HOME_CONTROLLER = "Home/";
        private const string PROJECT_CONTROLLER = "Projects/";
        private Claim _userAdministrationClaim = new Claim(ClaimTypes.Role, "Admin");
        private Claim _projectManagerClaim = new Claim(ClaimTypes.Role, "Manager");
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { ACCOUNT_CONTROLLER + "Update", new List<Claim>(), true };

            yield return new object[] { ADMIN_CONTROLLER + "Users", new List<Claim> { _userAdministrationClaim } };
            yield return new object[] { ADMIN_CONTROLLER + "GetUserClaims", new List<Claim> { _userAdministrationClaim }, true };
            
            yield return new object[] { HOME_CONTROLLER + "Index", new List<Claim>() };

            yield return new object[] { PROJECT_CONTROLLER + "Index", new List<Claim> { _projectManagerClaim } };
            yield return new object[] { PROJECT_CONTROLLER + "Manage", new List<Claim> { _projectManagerClaim }, true };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}