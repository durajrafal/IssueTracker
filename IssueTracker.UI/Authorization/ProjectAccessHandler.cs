using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Security.Claims;

namespace IssueTracker.UI.Authorization
{
    public class ProjectAccessHandler : AuthorizationHandler<ProjectAccessRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public ProjectAccessHandler(IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAccessRequirement requirement)
        {
            string? projectId = GetProjectIdFromRequest();
            if (projectId == null)
            {
                context.Fail();
                return;
            }
            if (context.User.HasClaim(AppClaimTypes.ProjectAccess, projectId))
            {
                context.Succeed(requirement);
            }
            else
            {
                var claims = await _userService.GetUserClaimsAsync(context.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var thisProjectAccessClaim = claims?.FirstOrDefault(x => x.Type == AppClaimTypes.ProjectAccess && x.Value == projectId);
                if (thisProjectAccessClaim is not null)
                {
                    await _userService.RefreshCurrentUserSignInAsync();
                    context.Succeed(requirement);
                }
            }

            return;

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
                if (httpContext.Request.RouteValues.TryGetValue("id", out projectId))
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
