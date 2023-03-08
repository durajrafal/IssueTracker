using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Projects.Commands.CreateProject
{
    public class CreateProjectCommand : IRequest<int>
    {
        public string Title { get; set; }
    }

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
    {
        private readonly IApplicationDbContext _ctx;

        public CreateProjectCommandHandler(IApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var entity = new Project
            {
                Title = request.Title,
            };

            _ctx.Projects.Add(entity);
            await _ctx.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
