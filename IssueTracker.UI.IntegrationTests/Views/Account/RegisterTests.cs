using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.UI.IntegrationTests.Views.Account
{
    public class RegisterTests : BaseTestWithScope
    {
        private const string REGISTER_URI = "/Identity/Account/Register";
        private RegisterViewModel _user;
        public RegisterTests()
            : base()
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
        public async void Register_WithRequiredFields_AddNewUserToDatabase()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            //Act
            var response = await client.SendFormAsync(HttpMethod.Post, REGISTER_URI, _user);

            //Assert
            var registeredUser = _testing.FuncDatabase<AuthDbContext, ApplicationUser>(ctx =>
            ctx.Users.Where(x => x.Email == _user.Email).First());
            Assert.NotNull(registeredUser);
            Assert.Equal(_user.Email, registeredUser.UserName);
            Assert.Equal(_user.Email, registeredUser.Email);
            Assert.Equal(_user.FirstName, registeredUser.FirstName);
            Assert.Equal(_user.LastName, registeredUser.LastName);
        }

        [Fact]
        public async void Register_WithoutField_NewUserNotAddedToDatabase()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            _user.FirstName = "";

            //Act
            var response = await client.SendFormAsync(HttpMethod.Post, REGISTER_URI, _user);

            //Assert
            var registeredUser = _testing.FuncDatabase<AuthDbContext, ApplicationUser>(ctx =>
            ctx.Users.Where(x => x.Email == _user.Email).FirstOrDefault());
            Assert.Null(registeredUser);
        }

        [Fact]
        public async void Register_NotUniqueEmail_NewUserNotAddedToDatabase()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            await _testing.ActionDatabaseAsync<AuthDbContext>( ctx => 
                ctx.Users.Add(new ApplicationUser(_user.Email, _user.FirstName, _user.LastName)));
            _user.FirstName = "Newname";

            //Act
            var response = await client.SendFormAsync(HttpMethod.Post, REGISTER_URI, _user);

            //Assert
            var registeredUser = _testing.FuncDatabase<AuthDbContext, ApplicationUser>(ctx =>
            ctx.Users.Where(x => x.Email == _user.Email).First());
            Assert.NotEqual(_user.FirstName, registeredUser.FirstName);
        }


    }
}
