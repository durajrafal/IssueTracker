using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Projects.Queries
{
    public class GetProjectDetailsQuery : IRequest<Project>
    {
        public int ProjectId { get; set; }
    }

    public class GetProjectDetailsQueryHandler : IRequestHandler<GetProjectDetailsQuery, Project>
    {
        private readonly IApplicationDbContext _ctx;

        public GetProjectDetailsQueryHandler(IApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<Project> Handle(GetProjectDetailsQuery request, CancellationToken cancellationToken)
        {
            return _ctx.Projects
                .Include(x => x.Members)
                .Include(x => x.Issues)
                .ThenInclude(y => y.Members)
                .FirstAsync(x => x.Id == request.ProjectId);
        }
    }
}
