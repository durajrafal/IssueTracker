using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Common.Models;
using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Areas.Identity.Controllers;
using IssueTracker.UI.Models.Account;
using IssueTracker.UI.Models.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.UI.IntegrationTests.Controllers
{
    public class AdminTests : UiBaseTest
    {
        public AdminTests(CustomWebApplicationFactory factory) 
            : base(factory)
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
            var model = response.Model as IEnumerable<UserDto>;

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
            var user = userManager.Users.First(x => x.FirstName == "Developer");
            var claims = await userManager.GetClaimsAsync(user);
            var vm = new UpdateUserRoleClaimViewModel(claims.ToList(), user.Id);
            vm.SelectedRole = newRoleClaimValue;
            userManager.Dispose();

            //Act
            await controller.UpdateUserRoleClaim(vm);

            //Arrange
            userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var updatedClaim = await userManager.GetClaimsAsync(user);
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
            var user = userManager.Users.First(x => x.FirstName == "Developer");
            var claims = await userManager.GetClaimsAsync(user);
            var vm = new UpdateUserRoleClaimViewModel(claims.ToList(), user.Id);
            var originalRole = vm.SelectedRole;
            vm.SelectedRole = "";

            //Act
            await controller.UpdateUserRoleClaim(vm);

            //Arrange
            var updatedClaim = await userManager.GetClaimsAsync(user);
            Assert.Equal(originalRole, updatedClaim.First(x => x.Type == ClaimTypes.Role).Value);
        }
    }
}
