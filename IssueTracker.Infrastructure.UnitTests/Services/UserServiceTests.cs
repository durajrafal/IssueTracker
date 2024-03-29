﻿using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Constants;
using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IssueTracker.Infrastructure.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        public UserServiceTests()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object, 
                Mock.Of<IHttpContextAccessor>(), 
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), 
                null, null, null);
            _mockCurrentUserService = new Mock<ICurrentUserService>();
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserFound_ShouldReturnThisUser()
        {
            //Arrange
            var appUser = new ApplicationUser("email@test.com", "Joe", "Doe");
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(appUser);
            IUserService userService = new UserService(_mockUserManager.Object, 
                _mockSignInManager.Object, _mockCurrentUserService.Object);

            //Act
            var user = await userService.GetUserByIdAsync("invalidId");

            //Assert
            user.Email.Should().Be(appUser.Email);
            user.FirstName.Should().Be(appUser.FirstName);
            user.LastName.Should().Be(appUser.LastName);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserNotFound_ShouldReturnNull()
        {
            //Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);
            IUserService userService = new UserService(_mockUserManager.Object,
                _mockSignInManager.Object, _mockCurrentUserService.Object);

            //Act
            var user = await userService.GetUserByIdAsync("invalidId");

            //Assert
            user.Should().BeNull();
        }

        [Fact]
        public async Task AddProjectAccessClaimToUserAsync_WhenUserExists_ShouldCallAddClaim()
        {
            //Arrange
            var projectId = 1;
            var appUser = new ApplicationUser("email@test.com", "Joe", "Doe");
            var claim = new Claim(AppClaimTypes.ProjectAccess, projectId.ToString());
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(appUser);
            _mockUserManager.Setup(x => x.AddClaimAsync(appUser, claim));
            IUserService userService = new UserService(_mockUserManager.Object, 
                _mockSignInManager.Object, _mockCurrentUserService.Object);

            //Act
            await userService.AddProjectAccessClaimToUserAsync(appUser.Id, projectId);

            //Assert
            _mockUserManager.Verify(x => 
                x.AddClaimAsync(appUser, It.Is<Claim>(x => x.Type == AppClaimTypes.ProjectAccess &&  x.Value == projectId.ToString())));
        }

        [Fact]
        public async Task RemoveProjectAccessClaimFromUserAsync_WhenUserExistsAndHasClaim_ShouldCallRemoveClaim()
        {
            //Arrange
            var projectId = 1;
            var appUser = new ApplicationUser("email@test.com", "Joe", "Doe");
            var claim = new Claim(AppClaimTypes.ProjectAccess, projectId.ToString());
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(appUser);
            _mockUserManager.Setup(x => x.GetClaimsAsync(appUser)).ReturnsAsync(new List<Claim>() { claim });
            _mockUserManager.Setup(x => x.RemoveClaimAsync(appUser, claim));
            IUserService userService = new UserService(_mockUserManager.Object, 
                _mockSignInManager.Object, _mockCurrentUserService.Object);

            //Act
            await userService.RemoveProjectAccessClaimFromUserAsync(appUser.Id, projectId);

            //Assert
            _mockUserManager.Verify(x => x.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()));
            _mockUserManager.Verify(x => x.RemoveClaimAsync(appUser, claim));
        }
    }
}