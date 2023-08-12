using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.UI.IntegrationTests.Views.Account
{
    public class UpdateTests : UiTestsFixture
    {
        UpdateViewModel _vm;
        private const string UPDATE_URI = "/Identity/Account/Update";
        public UpdateTests() : base()
        {
            AuthenticateFactory();
            _vm = new UpdateViewModel
            {
                Email = "mock@test.com",
                Password = "Pass123",
                ConfirmPassword = "Pass123",
                FirstName = "Registered",
                LastName = "User"
            };
        }

        [Fact]
        public async Task PostUpdate_WhenRequiredFieldsArePresentAndChanged_ShouldUpdateUser()
        {
            //Arrange
            var user = new ApplicationUser(_vm.Email, _vm.FirstName, _vm.LastName);
            using (var userManager = GetScopedService<UserManager<ApplicationUser>>())
            {
                var userId = Factory.Services.GetRequiredService<ICurrentUserService>().UserId;
                user.Id = userId;
                await userManager.CreateAsync(user, _vm.Password);
            }

            //Act
            _vm.FirstName = "Updated";
            _vm.LastName = "Von User";
            _vm.Password = "";
            _vm.ConfirmPassword = "";
            await Client.SendFormAsync(HttpMethod.Post, UPDATE_URI, _vm);

            //Assert
            ApplicationUser updatedUser;
            using (var userManager = GetScopedService<UserManager<ApplicationUser>>())
            {
                updatedUser = await userManager.FindByIdAsync(user.Id);
            }
            Assert.Equal(_vm.FirstName, updatedUser.FirstName);
            Assert.Equal(_vm.LastName, updatedUser.LastName);
        }

        [Fact]
        public async Task PostUpdate_WhenRequiredFieldIsNotPresent_ShouldNotUpdateUser()
        {
            //Arrange
            var user = new ApplicationUser(_vm.Email, _vm.FirstName, _vm.LastName);
            using (var userManager = GetScopedService<UserManager<ApplicationUser>>())
            {
                var userId = Factory.Services.GetRequiredService<ICurrentUserService>().UserId;
                user.Id = userId;
                await userManager.CreateAsync(user, _vm.Password);
            }

            //Act
            _vm.FirstName = "";
            await Client.SendFormAsync(HttpMethod.Post, UPDATE_URI, _vm);

            //Assert
            ApplicationUser updatedUser;
            using (var userManager = GetScopedService<UserManager<ApplicationUser>>())
            {
                updatedUser = await userManager.FindByIdAsync(user.Id);
            }
            Assert.NotEqual(_vm.FirstName, updatedUser.FirstName);
            Assert.Equal(user.FirstName, updatedUser.FirstName);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task PostUpdate_WhenPasswordIsChanged_ShouldUpdateOnlyWhenRequested(bool requested)
        {
            //Arrange
            var user = new ApplicationUser(_vm.Email, _vm.FirstName, _vm.LastName);
            using (var userManager = GetScopedService<UserManager<ApplicationUser>>())
            {
                var userId = Factory.Services.GetRequiredService<ICurrentUserService>().UserId;
                user.Id = userId;
                await userManager.CreateAsync(user, _vm.Password);
            }

            //Act
            _vm.PasswordChangeRequested = requested;
            _vm.Password = "Newpas123";
            _vm.ConfirmPassword = "Newpas123";
            await Client.SendFormAsync(HttpMethod.Post, UPDATE_URI, _vm);

            //Assert
            ApplicationUser updatedUser;
            using (var userManager = GetScopedService<UserManager<ApplicationUser>>())
            {
                updatedUser = await userManager.FindByIdAsync(user.Id);
            }
            if (requested)
                Assert.NotEqual(user.PasswordHash, updatedUser.PasswordHash);
            else
                Assert.Equal(user.PasswordHash, updatedUser.PasswordHash);
        }
    }
}
