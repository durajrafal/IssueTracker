using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Application.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        public UserServiceTests()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserFound_ShouldReturnThisUser()
        {
            //Arrange
            var appUser = new ApplicationUser("email@test.com", "Joe", "Doe");
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(appUser);
            IUserService userService = new UserService(_mockUserManager.Object);

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
            IUserService userService = new UserService(_mockUserManager.Object);

            //Act
            var user = await userService.GetUserByIdAsync("invalidId");

            //Assert
            user.Should().BeNull();
        }
    }
}
