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
    public class IndexTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public IndexTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(5)]
        public async Task Get_WhenUserLoggedIn_DisplayAssignedProjects(int numberOfProjects)
        {
            //Arrange
            var localFactory = _factory.MakeAuthenticated();
            var client = localFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);
            var userId = localFactory.Services.GetRequiredService<ICurrentUserService>().UserId;
            var testing = new TestingHelpers(localFactory);
            for (int i = 1; i <= numberOfProjects; i++)
            {
                var project = ProjectHelpers.CreateTestProject($"Project {i}");
                project.Members.Add(new ProjectMember { UserId = userId });
                await testing.ActionDatabaseAsync(ctx => ctx.Projects.Add(project));
            }

            //Act
            var page = await client.GetAsync("/");
            var pageHtml = await page.Content.ReadAsStringAsync();

            //Assert
            var projectsCount = testing.FuncDatabase(ctx => 
                ctx.Projects.Where(x => x.Members.Any(x => x.UserId == userId)).Count());
            var tableBody = pageHtml.Split("<tbody>").Last().Split("</tbody>").First();
            var rows = tableBody.Split("<tr").Skip(1);
            int tableRows = rows.Count();
            Assert.Equal(numberOfProjects, projectsCount);
            Assert.Equal(numberOfProjects, tableRows);
        }
    }
}
