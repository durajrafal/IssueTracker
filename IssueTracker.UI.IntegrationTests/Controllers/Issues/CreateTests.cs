using IssueTracker.UI.Controllers;
using IssueTracker.UI.IntegrationTests;
using IssueTracker.UI.Models.Issues;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using IssueTracker.Infrastructure.Identity;

namespace IssueTracker.UI.FunctionalTests.Controllers.Issues
{
    public class CreateTests : UiTestsFixture
    {
        private readonly IssuesController _controller;
        public CreateTests()
        {
            AuthenticateFactory();
            var httpContext = new DefaultHttpContext()
            { 
                RequestServices = ScopeFactory.CreateScope().ServiceProvider 
            };
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            _controller = new IssuesController()
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public async Task GetCreate_Always_ShouldReturnViewWithSeededModel()
        {
            //Arrange
            var project = await ProjectHelpers
                .CreateTestProject(nameof(GetCreate_Always_ShouldReturnViewWithSeededModel))
                .AddToDatabaseAsync(Database)
                .SeedDatabaseWithMembersUsersAsync(Database);

            //Act
            var response = await _controller.Create(project.Id) as ViewResult;
            var model = response.Model as CreateIssueViewModel;

            //Assert
            model.Members.Should().HaveCount(project.Members.Count);
            model.ProjectId.Should().Be(project.Id);
        }
    }
}
