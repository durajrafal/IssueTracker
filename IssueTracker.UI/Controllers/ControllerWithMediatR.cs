using FluentValidation;
using IssueTracker.Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    [Authorize]
    public abstract class ControllerWithMediatR : Controller
    {
        protected ISender Mediator => HttpContext.RequestServices.GetRequiredService<ISender>();

        protected IResult GetTypedResultBasedOnExceptionType(Exception e)
        {
            if (e is ValidationException ve)
                return TypedResults.BadRequest(ve.Errors.First().ErrorMessage);
            if (e is NotFoundException)
                return TypedResults.NotFound(e.Message);
            if (e is UnauthorizedAccessException)
                return TypedResults.Forbid(new AuthenticationProperties { });

            return TypedResults.BadRequest();
        }
    }
}
