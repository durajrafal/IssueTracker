using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Projects.Queries.GetProjects
{
    public class ProjectSummaryDto
    {
        public int Id { get; set; }
        [Display(Name = "Project Name")]
        public string Title { get; set; }
        [Display(Name = "Members")]
        public int NumberOfMembers { get; set; }
    }
}
