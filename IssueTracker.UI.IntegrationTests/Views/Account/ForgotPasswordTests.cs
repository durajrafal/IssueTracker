using Castle.Components.DictionaryAdapter.Xml;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.UI.IntegrationTests.Views.Account
{
    public class ForgotPasswordTests : UiTestsFixture
    {
        private const string FORGOT_URI = "/Identity/Account/ForgotPassword";
        private ForgotPasswordViewModel _vm;

        public ForgotPasswordTests() : base()
        {
            _vm = new ForgotPasswordViewModel
            {
                Email = "mock@test.com"
            };
        }

        [Fact]
        public async Task ForgotPassword_WhenEmailIsSent_ShouldRedirectToLoginPage()
        {
            //Arrange
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            //Act
            var response = await client.SendFormAsync(HttpMethod.Post, FORGOT_URI, _vm);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Identity", response.Headers.Location.OriginalString); 
        }

        [Fact]
        public async Task ForgotPassword_WhenEmailDoesntExist_ShouldRedirectToLoginPage()
        {
            //Arrange
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var user = new ApplicationUser(_vm.Email, "John", "Smith");
                await userManager.CreateAsync(user, "Pass123");
            }

            //Act
            var response = await client.SendFormAsync(HttpMethod.Post, FORGOT_URI, _vm);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Identity", response.Headers.Location.OriginalString); 
        }

        [Fact]
        public async Task ForgotPassword_WhenEmailExistsButFailsToSend_ShouldReturnSameView()
        {
            //Arrange
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            _vm.Email = "fail@test.com";
            using (var userManager = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var user = new ApplicationUser(_vm.Email, "John", "Smith");
                await userManager.CreateAsync(user, "Pass123");
            }

            //Act
            var response = await client.SendFormAsync(HttpMethod.Post, FORGOT_URI, _vm);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
