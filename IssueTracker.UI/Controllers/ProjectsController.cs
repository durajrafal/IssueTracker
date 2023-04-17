using IssueTracker.Application.Projects.Commands.CreateProject;
using IssueTracker.Application.Projects.Commands.Delete;
using IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment;
using IssueTracker.Application.Projects.Queries.GetProjects;
using IssueTracker.UI.Models.Projects;
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
        public async Task<IActionResult> Manage(int id)
        {
            var query = new GetProjectDetailsForManagmentQuery { ProjectId = id };
            var result = await Mediator.Send(query);
            var model = new ProjectManagmentViewModel { Title = result.Title, Members = result.Members, OtherUsers = result.OtherUsers };
            return View(model);
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
