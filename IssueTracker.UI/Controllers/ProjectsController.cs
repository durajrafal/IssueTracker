using IssueTracker.Application.Projects.Commands.CreateProject;
using IssueTracker.Application.Projects.Commands.Delete;
using IssueTracker.Application.Projects.Commands.UpdateProject;
using IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment;
using IssueTracker.Application.Projects.Queries.GetProjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    [Route("[controller]")]
    [Authorize(Policy = "ProjectManagement")]
    public class ProjectsController : CustomController
    {
        [HttpPost("")]
        public async Task<IActionResult> Create(string title)
        {
            var command = new CreateProjectCommand { Title = title };
            await Mediator.Send(command);
            return RedirectToAction("Index");
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var query = new GetProjectsQuery();
            var result = await Mediator.Send(query);
            return View(result);
        }

        [HttpGet("{id}/Manage")]
        public async Task<IActionResult> Manage(int id)
        {
            return View();
        }

        [HttpGet("~/api/[controller]/{id}/Manage")]
        public async Task<IActionResult> GetProjectForManage(int id)
        {
            var query = new GetProjectDetailsForManagmentQuery { ProjectId = id };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("~/api/[controller]/{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] UpdateProjectCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(int id, string title)
        {
            var command = new DeleteProjectCommand { ProjectId = id, Title = title };
            var result = await Mediator.Send(command);
            return RedirectToAction("Index");
        }
    }
}
