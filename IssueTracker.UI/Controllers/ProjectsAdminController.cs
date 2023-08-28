using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Commands.CreateProject;
using IssueTracker.Application.Projects.Commands.DeleteProject;
using IssueTracker.Application.Projects.Commands.UpdateProject;
using IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment;
using IssueTracker.Application.Projects.Queries.GetProjects;
using IssueTracker.UI.Models.Projects;
using IssueTracker.UI.Models.ProjectsAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    [Route("Project-Management")]
    [Authorize(Policy = "ProjectManagement")]
    public class ProjectsAdminController : ControllerWithMediatR
    {
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var query = new GetProjects();
            var result = await Mediator.Send(query);
            var vm = new ProjectsSummaryListViewModel { Projects = result.ToList() };
            return View(vm);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(ProjectsAdminCreateViewModel vm, [FromServices] IApplicationDbContext ctx)
        {
            var command = new CreateProject { Title = vm.Title };
            await Mediator.Send(command);
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "ProjectAccess")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            return View();
        }

        [Authorize(Policy = "ProjectAccess")]
        [HttpGet("~/api/project-management/{id}")]
        public async Task<IResult> GetDetails(int id)
        {
            var query = new GetProjectDetailsForManagment { ProjectId = id };
            ProjectManagmentDto result;
            try
            {
                result = await Mediator.Send(query);
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException && e.Message.Contains("Sequence contains no elements"))
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.BadRequest();
            }
            return TypedResults.Ok(result);
        }

        [Authorize(Policy = "ProjectAccess")]
        [HttpPut("~/api/project-management/{id}")]
        public async Task<IResult> Edit(int id, [FromBody] UpdateProject command)
        {
            try
            {
                await Mediator.Send(command);
            }
            catch (ValidationException ve)
            {
                return TypedResults.BadRequest(ve.Errors.First().ErrorMessage);
            }
            return TypedResults.Ok();
        }

        [Authorize(Policy = "ProjectAccess")]
        [HttpDelete("~/api/project-management/{id}/{title}")]
        public async Task<IResult> Delete(int id, string title)
        {
            var command = new DeleteProject { ProjectId = id, Title = title };
            try
            {
                await Mediator.Send(command);
            }
            catch (ValidationException ve)
            {
                return TypedResults.BadRequest(ve.Errors.First().ErrorMessage);
            }
            return TypedResults.Ok();
        }
    }
}
