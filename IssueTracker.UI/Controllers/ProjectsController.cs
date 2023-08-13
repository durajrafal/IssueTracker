using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Application.Projects.Queries.GetProjects;
using IssueTracker.UI.Models.Projects;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    public class ProjectsController : ControllerWithMediatR
    {
        [HttpGet("/project/{id}")]
        public async Task<IActionResult> Details (int id)
        {
            var query = new GetProjectDetailsQuery { ProjectId = id };
            var result = await Mediator.Send(query);
            var vm = new ProjectViewModel()
            {
                Id = result.Id,
                Title = result.Title,
                Issues = result.Issues
            };
            return View(vm);
        }
    }
}
