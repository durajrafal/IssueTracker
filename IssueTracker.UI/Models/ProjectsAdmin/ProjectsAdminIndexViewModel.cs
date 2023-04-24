using IssueTracker.Application.Projects.Queries.GetProjects;

namespace IssueTracker.UI.Models.ProjectsAdmin
{
    public class ProjectsAdminIndexViewModel
    {
        public IQueryable<ProjectSummaryDto> Projects { get; set; }
    }
}
