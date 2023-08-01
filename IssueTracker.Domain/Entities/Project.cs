using IssueTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IList<Member> Members { get; init; } = new List<Member>();
        public IList<Issue> Issues { get; set; } = new List<Issue>();
    }
}
