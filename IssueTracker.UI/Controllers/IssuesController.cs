using FluentValidation;
using IssueTracker.Application.Issues.Commands;
using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Domain.Entities;
using IssueTracker.UI.Filters;
using IssueTracker.UI.Models.Issues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.UI.Controllers
{
    [Authorize(Policy = "ProjectAccess")]
    [Route("Projects/{projectid}/[controller]")]
    public class IssuesController : ControllerWithMediatR
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> Create(int projectId)
        {
            var query = new GetProjectDetails { ProjectId = projectId };
            var result = await Mediator.Send(query);
            var projectMembersSelectListItems = new List<SelectListItem>();
            result.Members.ToList().ForEach(x =>
            {
                projectMembersSelectListItems.Add(new SelectListItem
                {
                    Text = $"{x.User.FullName} ({x.User.Email})",
                    Value = x.UserId
                });
            });
            var vm = new CreateIssueViewModel()
            {
                ProjectId = projectId,
                ProjectMembersSelecList = projectMembersSelectListItems,
            };
            return View(vm);
        }


        [HttpPost("[action]")]
        [ValidationExceptionFilter]
        public async Task<IActionResult> Create(CreateIssueViewModel model)
        {
            var members = new List<Member>();
            model.AssignedMembersId?.ToList().ForEach(x => members.Add(new Member() { UserId = x }));
            var command = new CreateIssue()
            {
                ProjectId = model.ProjectId,
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                Members = members
            };

            try
            {
                await Mediator.Send(command);
            }
            catch (ValidationException exception)
            {
                exception.Errors.ToList().ForEach(x => ModelState.AddModelError(x.PropertyName, x.ErrorMessage));
                return View(model);
            }
            return RedirectToAction("Details", "Projects", new { Id = model.ProjectId });
        }
    }

}
