using IssueTracker.Application.Projects.Commands.CreateProject;
using IssueTracker.Application.Projects.Commands.Delete;
using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Application.Projects.Queries.GetProjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    [Authorize(Policy = "ProjectManagement")]
    public class ProjectsController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Create(string title)
        {
            var command = new CreateProjectCommand { Title = title };
            await Mediator.Send(command);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var query = new GetProjectsQuery();
            var result = await Mediator.Send(query);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var query = new GetProjectDetailsQuery { ProjectId = id };
            var result = await Mediator.Send(query);
            return View("Details",result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string title)
        {
            var command = new DeleteProjectCommand { ProjectId = id, Title = title };
            var result = await Mediator.Send(command);
            return RedirectToAction("Index");
        }
    }
}
