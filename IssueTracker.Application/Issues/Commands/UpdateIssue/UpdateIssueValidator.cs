using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Common.Validators;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Issues.Commands.UpdateIssue
{
    public class UpdateIssueValidator : TitleValidator<UpdateIssue>
    {
        public UpdateIssueValidator(IApplicationDbContext ctx) : base(ctx)
        {
        }

        public override bool BeUniqueTitle(UpdateIssue model, string title)
        {
            return !_ctx.Projects.Include(x => x.Issues).First(x => x.Id == model.ProjectId)
                .Issues.Any(x => x.Title == title && x.Id != model.Id);
        }
    }
}
