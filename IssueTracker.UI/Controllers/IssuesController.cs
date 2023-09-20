using FluentValidation;
using IssueTracker.Application.Common.Models;
using IssueTracker.Application.Issues.Commands.CreateIssue;
using IssueTracker.Application.Issues.Commands.DeleteIssue;
using IssueTracker.Application.Issues.Commands.UpdateIssue;
using IssueTracker.Application.Issues.Queries.GetIssueDetails;
using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Domain.Entities;
using IssueTracker.UI.Models;
using IssueTracker.UI.Models.Issues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var vm = new CreateIssueViewModel(result.Members)
            {
                ProjectId = projectId
            };
            return View(vm);
        }


        [HttpPost("[action]")]
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

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Details(int id, int? page)
        {
            GetIssueDetails query;
            if (page is not null)
                query = new GetIssueDetails(id, page.Value);
            else
                query = new GetIssueDetails(id);

            var result = await Mediator.Send(query);

            var vm = new IssueViewModel()
            {
                Id = result.Id,
                Title = result.Title, 
                Description = result.Description,
                Priority = result.Priority,
                Status = result.Status,
                Project = result.Project,
                Members = result.Members,
                Audit = AuditViewModel.Create(result.Audit)
            };

            return View(vm);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Edit(int id)
        {
            var query = new GetIssueDetails(id);
            var result = await Mediator.Send(query);

            var vm = new EditIssueViewModel()
            {
                Id = result.Id,
                Title = result.Title,
                Description = result.Description,
                Priority = result.Priority,
                Status = result.Status,
                ProjectId = result.Project.Id,
            };

            return View(vm);
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> Edit(int id, EditIssueViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var query = new GetIssueDetails(id);
            var result = await Mediator.Send(query);

            var command = new UpdateIssue()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                Status = model.Status,
                ProjectId = model.ProjectId,
                Members = result.Members
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
            return RedirectToAction("Details", new { id = model.Id, projectId = model.ProjectId});
        }

        [HttpDelete("~/api/projects/{projectId}/issues/{id}")]
        public async Task<IResult> Delete(int id)
        {
            var command = new DeleteIssue(id);
            try
            {
                await Mediator.Send(command);
            }
            catch (Exception e)
            {
                return GetTypedResultBasedOnExceptionType(e);
            }

            return TypedResults.NoContent();
        }

        [HttpGet("~/api/projects/{projectId}/issues/{id}/members")]
        public async Task<IResult> Members(int id)
        {
            var queryIssue = new GetIssueDetails(id);
            IssueDto issue;
            try
            {
                issue = await Mediator.Send(queryIssue);
            }
            catch (Exception e)
            {
                return GetTypedResultBasedOnExceptionType(e);
            }

            var model = new IssueMembersModel()
            {
                Id = issue.Id,
                Members = issue.Members,
                OtherUsers = issue.Project.Members.Except(issue.Members).ToList()
            };

            return TypedResults.Ok(model);
        }

        [HttpPut("~/api/projects/{projectId}/issues/{id}/members")]
        public async Task<IResult> Members(int id, [FromBody] IssueMembersModel model)
        {
            if (id != model.Id)
                return TypedResults.BadRequest();

            var query = new GetIssueDetails(id);
            var result = await Mediator.Send(query);

            var command = new UpdateIssue()
            {
                Id = result.Id,
                Title = result.Title,
                Description = result.Description,
                Priority = result.Priority,
                Status = result.Status,
                ProjectId = result.Project.Id,
                Members = model.Members
            };
            try
            {
                var response = await Mediator.Send(command);
            }
            catch (Exception e)
            {
                return GetTypedResultBasedOnExceptionType(e);
            }

            return TypedResults.Ok();
        }
    }

}
