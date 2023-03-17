using IssueTracker.Application.Projects.Commands.CreateProject;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    public class ProjectsController : BaseController
    {

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [Authorize(Policy = "ProjectManagement")]
        [HttpPost]
        public async Task<IActionResult> Create(string title)
        {
            var command = new CreateProjectCommand { Title = title };
            await Mediator.Send(command);
            return Ok(View());
        }

    }
}
