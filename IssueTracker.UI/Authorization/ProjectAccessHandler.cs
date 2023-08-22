using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace IssueTracker.UI.Authorization
{
    public class ProjectAccessHandler : AuthorizationHandler<ProjectAccessRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectAccessHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAccessRequirement requirement)
        {
            string? projectId = GetProjectIdFromRequest();
            if (projectId == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            if (context.User.HasClaim(AppClaimTypes.ProjectAccess, projectId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        
        }
        
        private string? GetProjectIdFromRequest()
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                return null;
            }

            object? controller;
            if (!httpContext.Request.RouteValues.TryGetValue("controller", out controller))
            {
                return null;
            }

            object projectId;
            if (controller.ToString().ToLower().Contains("project"))
            {
                if(httpContext.Request.RouteValues.TryGetValue("id", out projectId))
                {
                    return projectId.ToString();
                };
                return null;
            }
            else
            {
                if (httpContext.Request.RouteValues.TryGetValue("projectId", out projectId))
                {
                    return projectId.ToString();
                }
                return null;
            }
        }
    }

}
