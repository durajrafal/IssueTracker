using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Areas.Identity.Controllers;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.UI.IntegrationTests.Controllers
{
    public class AccountTests : UiTestsFixture
    {

        public AccountTests() : base()
        {

        }

        [Theory]
        [InlineData("LoginAsDeveloper")]
        [InlineData("LoginAsManager")]
        [InlineData("LoginAsAdmin")]
        public async void Login_WhenLoggedInAsDemoUser_ShouldReturnIdentityCookie(string action)
        {
            //Act
            var response = await Client.GetAsync($"/Identity/Account/{action}");

            //Assert
            var cookie = response.Headers
                .Where(x => x.Key == "Set-Cookie")
                .SelectMany(x => x.Value.Select(v => v.Split(";").First())).First();
            Assert.Equal(".AspNetCore.Identity.Application", cookie.Split("=").First());
            Assert.NotEqual("", cookie.Split("=").Last());
        }

        [Fact]
        public async void Logout_Always_ShouldEmptyIdentityCookie()
        {
            //Act
            var response = await Client.GetAsync("/Identity/Account/Logout");

            //Assert
            var cookie = response.Headers
                .Where(x => x.Key == "Set-Cookie")
                .SelectMany(x => x.Value.Select(v => v.Split(";").First())).First();
            Assert.Equal(".AspNetCore.Identity.Application", cookie.Split("=").First());
            Assert.Equal(string.Empty, cookie.Split("=").Last());
        }

        [Fact]
        public async Task GetUpdate_WhenUserLoggedIn_ReturnFilledViewModel()
        {
            //Arrange
            AuthenticateFactory();
            var user = new ApplicationUser("updateuser@test.com", "Update", "User");
            var currentUserService = Factory.Services.GetRequiredService<ICurrentUserService>();
            var userId = currentUserService.UserId;
            user.Id = userId;
            var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var controller = new AccountController(userManager)
            {
                TempData = SetupTempData(),
            };
            await userManager.CreateAsync(user, "Pass123");

            //Act
            var response = await controller.Update(currentUserService) as ViewResult;
            userManager.Dispose();
            var model = response.Model as UpdateViewModel;
            
            //Assert
            Assert.Equal(user.Email, model.Email);
            Assert.Equal(user.FirstName, model.FirstName);
            Assert.Equal(user.LastName, actual: model.LastName);
        }



    }
}
