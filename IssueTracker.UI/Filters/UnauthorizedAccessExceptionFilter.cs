using IssueTracker.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IssueTracker.UI.Filters
{
    public class UnauthorizedAccessExceptionFilter : IExceptionFilter
    {
        private readonly IUserService _userService;

        public UnauthorizedAccessExceptionFilter(IUserService userService)
        {
            _userService = userService;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is UnauthorizedAccessException)
            {
                context.Result = new ForbidResult();
                context.ExceptionHandled = true;
                _userService.RefreshCurrentUserSignInAsync();
            }
        }
    }
}
