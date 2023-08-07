using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Commands.CreateProject;
using IssueTracker.Application.Projects.Commands.Delete;
using IssueTracker.Application.Projects.Commands.UpdateProject;
using IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment;
using IssueTracker.Application.Projects.Queries.GetProjects;
using IssueTracker.UI.Models.ProjectsAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    [Route("Project-Management")]
    [Authorize(Policy = "ProjectManagement")]
    public class ProjectsAdminController : CustomController
    {
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var query = new GetProjectsQuery();
            var result = await Mediator.Send(query);
            var model = new ProjectsAdminIndexViewModel { Projects = result };
            return View(model);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(ProjectsAdminCreateViewModel vm, [FromServices] IApplicationDbContext ctx)
        {
            var command = new CreateProjectCommand { Title = vm.Title };
            try
            {
                var result = await Mediator.Send(command);

            }
            catch (ValidationException e)
            {
                e.Errors.ToList().ForEach(x => ModelState.AddModelError(x.PropertyName, x.ErrorMessage));
            }
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            return RedirectToAction("Index");
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectCommand command)
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
