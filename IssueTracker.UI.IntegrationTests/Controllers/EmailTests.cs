using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using IssueTracker.UI.Areas.Identity.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace IssueTracker.UI.IntegrationTests.Controllers
{
    public class EmailTests : BaseTestWithScope
    {
        private EmailController _controller;
        public EmailTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _controller = new EmailController()
            {
                TempData = tempData
            };
        }

        [Fact]
        public async Task Confirm_WhenUserExistsAndTokenIsValid_ShouldConfirmEmail()
        {
            //Arrange
            var user = new ApplicationUser("confirm@test.com", "John", "Smith");
            string token;
            using (var userManager = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                await userManager.CreateAsync(user, "Pass123");
                token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            }

            //Act
            RedirectToActionResult result;
            using (var userManager = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var actionResult = await _controller.Confirm(token, user.Email, userManager);
                result = actionResult as RedirectToActionResult;
            }

            //Assert
            Assert.Equal("Account", result.ControllerName);
            Assert.Equal("Login", result.ActionName);
            Assert.NotNull(_controller.TempData["EmailActionSuccess"]);
        }

        [Fact]
        public async Task Confirm_WhenUserExistsButTokenIsInvalid_ShouldRedirectToRegister()
        {
            //Arrange
            var user = new ApplicationUser("confirmfail@test.com", "John", "Smith");
            string token = Guid.NewGuid().ToString();
            using (var userManager = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                await userManager.CreateAsync(user, "Pass123");
            }

            //Act
            RedirectToActionResult result;
            using (var userManager = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var actionResult = await _controller.Confirm(token, user.Email, userManager);
                result = actionResult as RedirectToActionResult;
            }

            //Assert
            Assert.Equal("Account", result.ControllerName);
            Assert.Equal("Register", result.ActionName);
            Assert.NotNull(_controller.TempData["EmailActionError"]);
        }

    }
}
