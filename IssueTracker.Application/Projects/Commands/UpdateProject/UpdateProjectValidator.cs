using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Common.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Projects.Commands.UpdateProject
{
    public class UpdateProjectValidator : TitleValidator<UpdateProject>
    {
        public UpdateProjectValidator(IApplicationDbContext ctx) : base(ctx)
        {
        }

        public override bool BeUniqueTitle(UpdateProject model, string title)
        {
            return !_ctx.Projects
                .Where(x => x.Id != model.Id)
                .Any(x => x.Title == title);
        }
    }
}
