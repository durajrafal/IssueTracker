using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Domain.Entities
{
    public class IssueMember
    {
        public int IssueId { get; set; }
        public Issue Issue { get; set; }
        public string UserId { get; set; }
    }
}
