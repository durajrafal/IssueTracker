﻿using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace IssueTracker.UI.FunctionalTests.Views.Account
{
    public class RegisterViewTests : UiTestsFixture
    {
        private const string REGISTER_URI = "/Identity/Account/Register";
        private RegisterViewModel _user;
        public RegisterViewTests() : base()
        {
            _user = new RegisterViewModel
            {
                Email = "mock@test.com",
                Password = "Pass123",
                ConfirmPassword = "Pass123",
                FirstName = "Registered",
                LastName = "User"
            };
        }

        [Fact]
        public async void Register_WhenRequiredFieldsArePresent_ShouldAddNewUserToDatabase()
        {
            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, REGISTER_URI, _user);

            //Assert
            var registeredUser = Database.Func<AuthDbContext, ApplicationUser>(ctx =>
            ctx.Users.Where(x => x.Email == _user.Email).First());
            Assert.NotNull(registeredUser);
            Assert.Equal(_user.Email, registeredUser.UserName);
            Assert.Equal(_user.Email, registeredUser.Email);
            Assert.Equal(_user.FirstName, registeredUser.FirstName);
            Assert.Equal(_user.LastName, registeredUser.LastName);
            //TODO - find a way to verify if mocked IEmailService was called
        }

        [Fact]
        public async void Register_WhenRequiredFieldIsMissing_ShouldNotAddNewUserToDatabase()
        {
            //Arrange
            _user.FirstName = "";

            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, REGISTER_URI, _user);

            //Assert
            var registeredUser = Database.Func<AuthDbContext, ApplicationUser>(ctx =>
            ctx.Users.Where(x => x.Email == _user.Email).FirstOrDefault());
            Assert.Null(registeredUser);
        }

        [Fact]
        public async void Register_WhenEmailIsNotUnique_ShouldNotAddNewUserToDatabase()
        {
            //Arrange
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var user = new ApplicationUser(_user.Email, _user.FirstName, _user.LastName);
                await userManager.CreateAsync(user, _user.Password);
            }
            _user.FirstName = "Newname";

            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, REGISTER_URI, _user);

            //Assert
            var registeredUser = Database.Func<AuthDbContext, ApplicationUser>(ctx =>
                ctx.Users.Where(x => x.Email == _user.Email).First());
            Assert.NotEqual(_user.FirstName, registeredUser.FirstName);
        }


        [Fact]
        public async void Register_WhenFailsToSendAnEmail_ShouldNotAddNewUserToDatabase()
        {
            //Arrange
            _user.Email = "fail@test.com";

            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, REGISTER_URI, _user);

            //Assert
            var registeredUser = Database.Func<AuthDbContext, ApplicationUser>(ctx =>
                ctx.Users.Where(x => x.Email == _user.Email).FirstOrDefault());
            Assert.Null(registeredUser);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
