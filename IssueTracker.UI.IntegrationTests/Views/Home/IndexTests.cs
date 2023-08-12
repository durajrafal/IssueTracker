using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.IntegrationTests.Projects.Queries;
using IssueTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.UI.IntegrationTests.Views.Home
{
    public class IndexTests : UiTestsFixture
    {
        public IndexTests() : base()
        {
            AuthenticateFactory();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(5)]
        public async Task Get_WhenUserLoggedIn_ShouldDisplayAssignedProjects(int numberOfProjects)
        {
            //Arrange
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);
            var userId = Factory.Services.GetRequiredService<ICurrentUserService>().UserId;
            for (int i = 1; i <= numberOfProjects; i++)
            {
                var project = ProjectHelpers.CreateTestProject($"Project {i}");
                project.Members.Add(new Member { UserId = userId });
                await Database.ActionAsync(ctx => ctx.Projects.Add(project));
            }

            //Act
            var page = await Client.GetAsync("/");
            var pageHtml = await page.Content.ReadAsStringAsync();

            //Assert
            var projectsCount = Database.Func(ctx => 
                ctx.Projects.Where(x => x.Members.Any(x => x.UserId == userId)).Count());
            var tableBody = pageHtml.Split("<tbody>").Last().Split("</tbody>").First();
            var rows = tableBody.Split("<tr").Skip(1);
            int tableRows = rows.Count();
            Assert.Equal(numberOfProjects, projectsCount);
            Assert.Equal(numberOfProjects, tableRows);
        }
    }
}
