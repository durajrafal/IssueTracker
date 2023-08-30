using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Constants;
using IssueTracker.Domain.Entities;
using IssueTracker.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Net;
using System.Security.Claims;

namespace IssueTracker.UI.FunctionalTests.Views
{
    public class AuthTests : UiTestsFixture
    {
        public AuthTests() : base()
        {
        }

        [Theory]
        [ClassData(typeof(AuthorizeTestData))]
        public async Task GetEndpoint_WhenAuthorized_ShouldReturnEndpoint(string uri, List<Claim> claims)
        {
            //Arrange
            AuthenticateFactory(claims);
            var updatedUri = await PrepareApplicationForTestAsync(uri);

            //Act
            var response = await Client.GetAsync(updatedUri);

            //Assert 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Theory]
        [ClassData(typeof(AuthorizeTestData))]
        public async Task GetEndpoint_WhenNotAuthorized_ShouldNotReturnEndpoint(string uri, List<Claim> claims)
        {
            //Arrange
            bool hasClaim = claims.Count > 0;
            if (hasClaim)
            {
                AuthenticateFactory();
            }

            //Act
            var response = await Client.GetAsync(uri);

            //Assert
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
            //Assert
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

        private async Task<string> PrepareApplicationForTestAsync(string uri)
        {
            if (uri.Contains("1"))
            {
                var project = ProjectHelpers.CreateTestProject(nameof(AuthTests), GetCurrentUserId());
                project.Id = 1;
                await project.AddToDatabaseAsync(Database);
            }
            if (uri.StartsWith("Identity"))
            {
                var currentUserService = Factory.Services.GetRequiredService<ICurrentUserService>();
                var appUser = new ApplicationUser("email@test.com", "Current", "User");
                appUser.Id = currentUserService.UserId;
                await Database.ActionAsync<AuthDbContext>(ctx => ctx.Users.AddAsync(appUser));
                var userClaim = new Microsoft.AspNetCore.Identity.IdentityUserClaim<string>()
                {
                    UserId = appUser.Id,
                    ClaimType = ClaimTypes.Role,
                    ClaimValue = "Developer"
                };
                await Database.ActionAsync<AuthDbContext>(ctx => ctx.UserClaims.AddAsync(userClaim));
                if (uri.EndsWith("GetUserClaims/"))
                {
                    return uri + currentUserService.UserId;
                }
            }

            return uri;
        }
    }

    public class AuthorizeTestData : IEnumerable<object[]>
    {
        private const string ProjectId = "1";
        private const string ACCOUNT_CONTROLLER = "Identity/Account/";
        private const string ADMIN_CONTROLLER = "Identity/Admin/";
        private const string HOME_CONTROLLER = "Home/";
        private const string PROJECTS_ADMIN_CONTROLLER = "Project-Management/";
        private const string PROJECTS_ADMIN_API = "/api/project-management/";
        private const string PROJECTS_CONTROLLER = "Projects/";
        private const string ISSUES_CONTROLLER = $"Projects/{ProjectId}/Issues/";
        private Claim _userAdministrationClaim = new Claim(ClaimTypes.Role, "Admin");
        private Claim _projectManagerClaim = new Claim(ClaimTypes.Role, "Manager");
        private Claim _projectAccessClaim = new Claim(AppClaimTypes.ProjectAccess, ProjectId);
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { ACCOUNT_CONTROLLER + "Update", new List<Claim>() };

            yield return new object[] { ADMIN_CONTROLLER + "Users", new List<Claim> { _userAdministrationClaim } };
            yield return new object[] { ADMIN_CONTROLLER + "GetUserClaims/", new List<Claim> { _userAdministrationClaim } };

            yield return new object[] { HOME_CONTROLLER + "Index", new List<Claim>() };

            yield return new object[] { PROJECTS_ADMIN_CONTROLLER + "", new List<Claim> { _projectManagerClaim } };
            yield return new object[] { PROJECTS_ADMIN_CONTROLLER + ProjectId, new List<Claim> { _projectManagerClaim, _projectAccessClaim } };
            yield return new object[] { PROJECTS_ADMIN_API + ProjectId, new List<Claim> { _projectManagerClaim, _projectAccessClaim } };

            yield return new object[] { PROJECTS_CONTROLLER + ProjectId, new List<Claim> { _projectAccessClaim } };

            yield return new object[] { ISSUES_CONTROLLER + "Create", new List<Claim> { _projectAccessClaim } };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}