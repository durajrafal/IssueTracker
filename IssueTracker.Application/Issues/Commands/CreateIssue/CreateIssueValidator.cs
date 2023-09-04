using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Common.Validators;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Issues.Commands.CreateIssue
{
    public class CreateIssueValidator : TitleValidator<CreateIssue>
    {
        public CreateIssueValidator(IApplicationDbContext ctx) : base(ctx)
        {
        }

        public override bool BeUniqueTitle(CreateIssue model, string title)
        {
            return !_ctx.Projects.Include(x => x.Issues).First(x => x.Id == model.ProjectId)
                .Issues.Any(x => x.Title == title);
        }
    }
}
