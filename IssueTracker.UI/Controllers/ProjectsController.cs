using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.UI.Models.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    [Route("[controller]")]
    public class ProjectsController : ControllerWithMediatR
    {
        [HttpGet("{id}")]
        [Authorize(Policy = "ProjectAccess")]
        public async Task<IActionResult> Details(int id, string? orderBy, string status = "all")
        {
            var query = new GetProjectDetails { ProjectId = id };
            var result = await Mediator.Send(query);
            var vm = new ProjectViewModel()
            {
                Id = result.Id,
                Title = result.Title,
                Issues = result.Issues,
                SelectedStatus = status
            };
            return View(vm);
        }
    }
}
