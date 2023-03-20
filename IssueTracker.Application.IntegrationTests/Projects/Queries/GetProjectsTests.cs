using IssueTracker.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Queries.GetProjects;

namespace IssueTracker.Application.IntegrationTests.Projects.Queries
{
    public class GetProjectsTests : BaseTest
    {
        public GetProjectsTests()
            :base()
        {
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Handle_Always_GetProjectsOnlyWithUserAsMember(int numberOfUserProject)
        {
            var loggedUserId = _factory.Services.GetRequiredService<ICurrentUserService>().UserId;
            var testProjects = CreateTestProjects(numberOfUserProject, loggedUserId);
            await _testing.ActionDatabaseAsync(async ctx =>
                await ctx.AddRangeAsync(testProjects)
            );
            
            var query = new GetProjectsQuery();
            var projects = await _testing.MediatorSendAsync(query);

            Assert.Equal(numberOfUserProject, projects.Count());
        }

        private List<Project> CreateTestProjects(int numberOfUserProjects, string userId)
        {
            var output = new List<Project>();

            var otherMember = new ProjectMember { UserId = Guid.NewGuid().ToString() };

            for (int i = 1; i <= numberOfUserProjects; i++)
            {
                var projectTitle = $"Project {i}";
                output.Add(new Project { 
                    Title = projectTitle, 
                    Members = new List<ProjectMember> { 
                        new ProjectMember { UserId = userId } 
                    } 
                });
            }
            output.Add(new Project { Title = "Other project", Members = new List<ProjectMember> { otherMember } });

            return output;
        }
    }
}
