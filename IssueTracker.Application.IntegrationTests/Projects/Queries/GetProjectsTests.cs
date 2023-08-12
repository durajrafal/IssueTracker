using IssueTracker.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Queries.GetProjects;

namespace IssueTracker.Application.IntegrationTests.Projects.Queries
{
    public class GetProjectsTests : TestsWithMediatorFixture
    {
        public GetProjectsTests()
            :base()
        {
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Handle_Always_ShouldGetProjectsOnlyWithUserAsMember(int numberOfUserProject)
        {
            var loggedUserId = Factory.Services.GetRequiredService<ICurrentUserService>().UserId;
            var testProjects = CreateTestProjects(numberOfUserProject, loggedUserId);
            await Database.ActionAsync(async ctx =>
                await ctx.AddRangeAsync(testProjects)
            );
            
            var query = new GetProjectsQuery();
            var projects = await Mediator.Send(query);

            Assert.Equal(numberOfUserProject, projects.Count());
        }

        private List<Project> CreateTestProjects(int numberOfUserProjects, string userId)
        {
            var output = new List<Project>();

            var otherMember = new Member { UserId = Guid.NewGuid().ToString() };

            for (int i = 1; i <= numberOfUserProjects; i++)
            {
                var projectTitle = $"Project {i}";
                output.Add(new Project { 
                    Title = projectTitle, 
                    Members = new List<Member> { 
                        new Member { UserId = userId } 
                    } 
                });
            }
            output.Add(new Project { Title = "Other project", Members = new List<Member> { otherMember } });

            return output;
        }
    }
}
