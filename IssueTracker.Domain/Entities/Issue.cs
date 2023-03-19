using IssueTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Domain.Entities
{
    public class Issue
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.None;
        public IList<IssueMember> Members { get; init; } = new List<IssueMember>();
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
