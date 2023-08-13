using IssueTracker.Application.Projects.Queries.GetProjects;

namespace IssueTracker.UI.Models.Projects
{
    public class ProjectsSummaryListViewModel
    {
        public IReadOnlyList<ProjectSummaryDto> Projects { get; set; }
    }
}
