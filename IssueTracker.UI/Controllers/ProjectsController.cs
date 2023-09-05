using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Domain.Enums;
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
        public async Task<IActionResult> Details(int id, string? orderBy, string? status)
        {
            var query = new GetProjectDetails { ProjectId = id };
            var result = await Mediator.Send(query);
            WorkingStatus statusParsed;

            var vm = new ProjectViewModel()
            {
                Id = result.Id,
                Title = result.Title,
                OrderBy = orderBy
            };

            if (Enum.TryParse(status, out statusParsed))
            {
                vm.SelectedStatus = statusParsed.ToString();
                vm.Issues = result.Issues.Where(x => x.Status == statusParsed).ToList();
            }
            else
            {
                vm.Issues = result.Issues;
            }

            if (string.IsNullOrEmpty(orderBy))
                return View(vm);

            switch (orderBy)
            {
                case ProjectViewModel.PRIORITY:
                    vm.Issues = vm.Issues.OrderByDescending(x => x.Priority).ToList();
                    break;
                case ProjectViewModel.DATE:
                    //TODO - change it to date when it's added
                    vm.Issues = vm.Issues.OrderByDescending(x => x.Id).ToList();
                    break;
                default:
                    break;
            }

            return View(vm);
        }
    }
}
