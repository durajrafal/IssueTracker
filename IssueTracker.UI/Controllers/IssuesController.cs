using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.UI.Models.Issues;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    [Route("Projects/{projectid}/[controller]")]
    public class IssuesController: ControllerWithMediatR
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> Create(int projectId)
        {
            var query = new GetProjectDetails { ProjectId = projectId };
            var result = await Mediator.Send(query);
            var vm = new CreateIssueViewModel()
            {
                ProjectId = projectId,
                Members = result.Members,
            };
            return View(vm);
        }

    }
}
