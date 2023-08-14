using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Projects.Commands.DeleteProject
{
    public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
    {
        private readonly IApplicationDbContext _ctx;

        public DeleteProjectCommandValidator(IApplicationDbContext ctx)
        {
            var id =
            _ctx = ctx;
            RuleFor(x => x.Title)
                .Must((request, title) => MatchWithProject(request.ProjectId, title))
                .WithMessage("Entered title doesn't match title of the project you want to delete.");
        }

        public bool MatchWithProject(int id, string title)
        {
            return _ctx.Projects.FirstOrDefault(x => x.Id == id)?.Title == title;
        }
    }
}
