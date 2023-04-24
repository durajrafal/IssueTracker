using FluentValidation;
using IssueTracker.Application.Common.Interfaces;

namespace IssueTracker.Application.Projects.Commands.CreateProject
{
    public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
    {
        private readonly IApplicationDbContext _ctx;

        public CreateProjectCommandValidator(IApplicationDbContext ctx)
        {
            _ctx = ctx;
            const int MAX_TITLE_LENGTH = 100;
            var forbiddenCharacters = new List<char> { ':', '/', '?', '#', '[', ']', '@', '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=' };
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(MAX_TITLE_LENGTH).WithMessage($"Title must not exceed {MAX_TITLE_LENGTH}.")
                .Must(title => title.All(ch => !forbiddenCharacters.Contains(ch)))
                .WithMessage("Must not contain any of the following characters: ':', '/', '?', '#', '[', ']', '@', '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '='")
                .Must(BeUniqueTitle).WithMessage("Project with the same name already exists.");
        }

        public bool BeUniqueTitle(string title)
        {
            return !_ctx.Projects.Any(x => x.Title == title);
        }

    }
}
