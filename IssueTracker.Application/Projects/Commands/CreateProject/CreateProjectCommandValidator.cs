using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Common.Validators;

namespace IssueTracker.Application.Projects.Commands.CreateProject
{
    public class CreateProjectCommandValidator : TitleValidator<CreateProjectCommand>
    {
        public CreateProjectCommandValidator(IApplicationDbContext ctx) :base(ctx)
        {    
        }

        public override bool BeUniqueTitle(CreateProjectCommand model, string title)
        {
            return !_ctx.Projects.Any(x => x.Title == title);
        }
    }
}
