using IssueTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Projects.Queries.GetProjectDetails
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<Member> Members { get; set; }
        public ICollection<Issue> Issues { get; set; }
    }
}
