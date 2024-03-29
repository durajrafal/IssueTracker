﻿using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using IssueTracker.UI.Areas.Identity.Controllers;
using IssueTracker.UI.Models.Account;

namespace IssueTracker.UI.FunctionalTests.Controllers
{
    public class EmailControllerTests : UiTestsFixture
    {
        private EmailController _controller;
        private const string PASSWORD = "Pass123";
        public EmailControllerTests() : base()
        {
            _controller = new EmailController()
            {
                TempData = SetupTempData()
            };
        }

        [Fact]
        public async Task Confirm_WhenUserExistsAndTokenIsValid_ShouldConfirmEmail()
        {
            //Arrange
            var user = new ApplicationUser("confirm@test.com", "John", "Smith");
            string token;
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                await userManager.CreateAsync(user, PASSWORD);
                token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            }

            //Act
            RedirectToActionResult result;
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
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
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                await userManager.CreateAsync(user, PASSWORD);
            }

            //Act
            RedirectToActionResult result;
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var actionResult = await _controller.Confirm(token, user.Email, userManager);
                result = actionResult as RedirectToActionResult;
            }

            //Assert
            Assert.Equal("Account", result.ControllerName);
            Assert.Equal("Register", result.ActionName);
            Assert.NotNull(_controller.TempData["EmailActionError"]);
        }

        [Fact]
        public async Task ResetPassword_WhenUserExistsAndTokenIsValid_ShouldChangePassword()
        {
            //Arrange
            var user = new ApplicationUser("reset@test.com", "John", "Smith");
            string token;
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                await userManager.CreateAsync(user, PASSWORD);
                token = await userManager.GeneratePasswordResetTokenAsync(user);
            }

            //Act
            RedirectToActionResult result;
            var vm = new ResetPasswordViewModel
            {
                Email = user.Email,
                Token = token,
                Password = "Newpass123",
                ConfirmPassword = "Newpass123",
            };

            bool isPasswordChanged;
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var actionResult = await _controller.ResetPassword(vm, userManager);
                result = actionResult as RedirectToActionResult;
                user = await userManager.FindByEmailAsync(vm.Email);
                isPasswordChanged = await userManager.CheckPasswordAsync(user, vm.Password);
            }

            //Assert
            Assert.Equal("Account", result.ControllerName);
            Assert.Equal("Login", result.ActionName);
            Assert.NotNull(_controller.TempData["EmailActionSuccess"]);
            Assert.True(isPasswordChanged);
        }
        
        [Fact]
        public async Task ResetPassword_WhenUserExistsAndTokenIsInvalid_ShouldNotChangePassword()
        {
            //Arrange
            var user = new ApplicationUser("resetfail@test.com", "John", "Smith");
            string token = Guid.NewGuid().ToString();
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                await userManager.CreateAsync(user, PASSWORD);
            }

            //Act
            ViewResult result;
            var vm = new ResetPasswordViewModel
            {
                Email = user.Email,
                Token = token,
                Password = "Newpass123",
                ConfirmPassword = "Newpass123",
            };

            bool isPasswordChanged, isOldPasswordValid;
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var actionResult = await _controller.ResetPassword(vm, userManager);
                result = actionResult as ViewResult;
                user = await userManager.FindByEmailAsync(vm.Email);
                isPasswordChanged = await userManager.CheckPasswordAsync(user, vm.Password);
                isOldPasswordValid = await userManager.CheckPasswordAsync(user, PASSWORD);
            }

            //Assert
            Assert.NotNull(result.Model);
            Assert.Equal(1, _controller.ModelState.ErrorCount);
            Assert.False(isPasswordChanged);
            Assert.True(isOldPasswordValid);
        }
    }
}
