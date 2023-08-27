using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace IssueTracker.UI.FunctionalTests.Views.Account
{
    public class ForgotPasswordViewTests : UiTestsFixture
    {
        private const string FORGOT_URI = "/Identity/Account/ForgotPassword";
        private ForgotPasswordViewModel _vm;

        public ForgotPasswordViewTests() : base()
        {
            _vm = new ForgotPasswordViewModel
            {
                Email = "mock@test.com"
            };
        }

        [Fact]
        public async Task ForgotPassword_WhenEmailIsSent_ShouldRedirectToLoginPage()
        {
            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, FORGOT_URI, _vm);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Identity", response.Headers.Location.OriginalString); 
        }

        [Fact]
        public async Task ForgotPassword_WhenEmailDoesntExist_ShouldRedirectToLoginPage()
        {
            //Arrange
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var user = new ApplicationUser(_vm.Email, "John", "Smith");
                await userManager.CreateAsync(user, "Pass123");
            }

            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, FORGOT_URI, _vm);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Identity", response.Headers.Location.OriginalString); 
        }

        [Fact]
        public async Task ForgotPassword_WhenEmailExistsButFailsToSend_ShouldReturnSameView()
        {
            //Arrange
            _vm.Email = "fail@test.com";
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var user = new ApplicationUser(_vm.Email, "John", "Smith");
                await userManager.CreateAsync(user, "Pass123");
            }

            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, FORGOT_URI, _vm);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
