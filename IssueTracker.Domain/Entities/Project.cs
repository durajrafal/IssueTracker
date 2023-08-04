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
        public ICollection<Member> Members { get; init; } = new List<Member>();
        public ICollection<Issue> Issues { get; init; } = new List<Issue>();
    }
}
