using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Domain.Enums;
using IssueTracker.UI.Models;
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
                OrderBy = orderBy,
                Audit = AuditViewModel.Create(result.Audit)
            };

            if (Enum.TryParse(status, out statusParsed))
            {
                vm.SelectedStatus = statusParsed.ToString();
                vm.Issues = result.Issues?.Where(x => x.Status == statusParsed).ToList();
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
                    vm.Issues = vm.Issues?.OrderByDescending(x => x.Priority).ToList();
                    break;
                case ProjectViewModel.DATE_CREATED:
                    vm.Issues = vm.Issues?.OrderBy(x => x.Audit.Created).ToList();
                    break;                
                case ProjectViewModel.DATE_CREATED_DESC:
                    vm.Issues = vm.Issues?.OrderByDescending(x => x.Audit.Created).ToList();
                    break;
                case ProjectViewModel.DATE_MODIFIED:
                    vm.Issues = vm.Issues?.OrderBy(x => x.Audit.LastModified).ToList();
                    break;
                case ProjectViewModel.DATE_MODIFIED_DESC:
                    vm.Issues = vm.Issues?.OrderByDescending(x => x.Audit.LastModified).ToList();
                    break;
                default:
                    break;
            }

            return View(vm);
        }
    }
}
