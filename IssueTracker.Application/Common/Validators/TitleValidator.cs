﻿using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Common.Validators
{
    public abstract class TitleValidator<T>: AbstractValidator<T> where T: IHasTitle
    {
        protected readonly IApplicationDbContext _ctx;

        public TitleValidator(IApplicationDbContext ctx)
        {
            _ctx = ctx;
            const int MAX_TITLE_LENGTH = 100;
            var forbiddenCharacters = new List<char> { ':', '/', '?', '#', '[', ']', '@', '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=' };
            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotNull().NotEmpty().WithMessage("Title is required.")
                .MaximumLength(MAX_TITLE_LENGTH).WithMessage($"Title must not exceed {MAX_TITLE_LENGTH}.")
                .Must(title => title.All(ch => !forbiddenCharacters.Contains(ch)))
                .WithMessage("Must not contain any of the following characters: ':', '/', '?', '#', '[', ']', '@', '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '='")
                .Must(BeUniqueTitle).WithMessage("Project with the same name already exists.");
            _ctx = ctx;
        }

        public abstract bool BeUniqueTitle(T model, string title);
    }
}
