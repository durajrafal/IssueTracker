using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Projects.Commands.Delete
{
    public class DeleteProjectCommand : IRequest<int>, IHasTitle
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
    }

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, int>
    {
        private readonly IApplicationDbContext _ctx;

        public DeleteProjectCommandHandler(IApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<int> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var entity = _ctx.Projects.FirstOrDefault(x => x.Id == request.ProjectId);
            _ctx.Projects.Remove(entity);
            return await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
