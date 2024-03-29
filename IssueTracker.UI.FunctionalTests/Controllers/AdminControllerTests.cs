﻿using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Areas.Identity.Controllers;
using IssueTracker.UI.Models.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace IssueTracker.UI.FunctionalTests.Controllers
{
    public class AdminControllerTests : UiTestsFixture
    {
        public AdminControllerTests() : base()
        {

        }

        [Fact]
        public async Task Users_Always_ShouldReturnAllUsers()
        {
            //Arrange
            var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userService = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUserService>();

            var controller = new AdminController(userService)
            {
                TempData = SetupTempData(),
            };

            //Act
            var response = await controller.Users() as ViewResult;
            var model = response.Model as IEnumerable<UserAdminViewModel>;

            //Arrange
            Assert.NotNull( model);
            Assert.Equal(userManager.Users.Count(), model.Count());
        }

        [Theory]
        [InlineData("Developer")]
        [InlineData("Manager")]
        [InlineData("Admin")]
        public async Task UpdateUserRoleClaim_WhenNewSelected_ShouldUpdate(string newRoleClaimValue)
        {
            //Arrange
            var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userService = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUserService>();

            var controller = new AdminController(userService)
            {
                TempData = SetupTempData(),
            };
            var identityUser = userManager.Users.First(x => x.FirstName == "Developer");
            var user = await userService.GetUserByIdAsync(identityUser.Id);
            var roleClaim = await userService.GetUserRoleClaimAsync(user!.UserId);
            var vm = new UpdateUserRoleClaimViewModel(roleClaim!, user);
            vm.SelectedRole = newRoleClaimValue;
            userManager.Dispose();

            //Act
            await controller.UpdateUserRoleClaim(vm);

            //Arrange
            userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var updatedClaim = await userManager.GetClaimsAsync(identityUser);
            Assert.Equal(vm.SelectedRole, updatedClaim.First(x => x.Type == ClaimTypes.Role).Value);
        }

        [Fact]
        public async Task UpdateUserRoleClaim_WhenNewNotSelected_ShouldNotUpdate()
        {
            //Arrange
            var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userService = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUserService>();

            var controller = new AdminController(userService)
            {
                TempData = SetupTempData(),
            };
            var identityUser = userManager.Users.First(x => x.FirstName == "Developer");
            var user = await userService.GetUserByIdAsync(identityUser.Id);
            var roleClaim = await userService.GetUserRoleClaimAsync(user!.UserId);
            var vm = new UpdateUserRoleClaimViewModel(roleClaim!, user);
            var originalRole = vm.SelectedRole;
            vm.SelectedRole = "";

            //Act
            await controller.UpdateUserRoleClaim(vm);

            //Arrange
            var updatedClaim = await userManager.GetClaimsAsync(identityUser);
            Assert.Equal(originalRole, updatedClaim.First(x => x.Type == ClaimTypes.Role).Value);
        }
    }
}
