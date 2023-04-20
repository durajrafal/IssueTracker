using IssueTracker.Application.Projects.Commands.CreateProject;
using IssueTracker.Application.Projects.Commands.Delete;
using IssueTracker.Application.Projects.Commands.UpdateProject;
using IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment;
using IssueTracker.Application.Projects.Queries.GetProjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    [Route("project-management")]
    [Authorize(Policy = "ProjectManagement")]
    public class ProjectsAdminController : CustomController
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            return View();
        }

        [HttpGet("~/api/project-management/{id}")]
        public async Task<IActionResult> GetProjectForManage(int id)
        {
            var query = new GetProjectDetailsForManagmentQuery { ProjectId = id };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("~/api/project-management/{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] UpdateProjectCommand command)
        {
            await Mediator.Send(command);
            return Ok();
        }

        [HttpDelete("~/api/project-management/{id}/{title}")]
        public async Task<IActionResult> Delete(int id, string title)
        {
            var command = new DeleteProjectCommand { ProjectId = id, Title = title };
            await Mediator.Send(command);
            return Ok();
        }
    }
}
