using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Common.Validators;

namespace IssueTracker.Application.Projects.Commands.CreateProject
{
    public class CreateProjectValidator : TitleValidator<CreateProject>
    {
        public CreateProjectValidator(IApplicationDbContext ctx) :base(ctx)
        {    
        }

        public override bool BeUniqueTitle(CreateProject model, string title)
        {
            return !_ctx.Projects.Any(x => x.Title == title);
        }
    }
}
