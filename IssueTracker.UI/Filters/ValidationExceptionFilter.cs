using FluentValidation;
using IssueTracker.UI.Models.ProjectsAdmin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;

namespace IssueTracker.UI.Filters
{
    public class ValidationExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException validationException)
            {
                validationException.Errors.ToList().ForEach(e => context.ModelState.AddModelError(e.PropertyName, e.ErrorMessage));

                context.Result = new ViewResult();

                context.ExceptionHandled = true;
            }
        }
    }
}
