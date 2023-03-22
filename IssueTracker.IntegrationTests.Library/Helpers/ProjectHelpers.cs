using IssueTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.IntegrationTests.Library.Helpers
{
    public static class ProjectHelpers
    {
        public static Project CreateTestProject(string title)
        {
            var projectMember1 = new ProjectMember { UserId = Guid.NewGuid().ToString() };
            var projectMember2 = new ProjectMember { UserId = Guid.NewGuid().ToString() };

            var issueMember1 = new IssueMember { UserId = projectMember1.UserId };
            var issueMember2 = new IssueMember { UserId = projectMember2.UserId };

            var issue1 = new Issue
            {
                Title = "Issue 1",
                Members = new List<IssueMember> { issueMember1 },
            };

            var issue2 = new Issue
            {
                Title = "Issue 2",
                Members = new List<IssueMember> { issueMember2 },
            };

            return new Project
            {
                Title = title,
                Members = new List<ProjectMember> { projectMember1, projectMember2 },
                Issues = new List<Issue> { issue1, issue2 }
            };
        }
    }
}
