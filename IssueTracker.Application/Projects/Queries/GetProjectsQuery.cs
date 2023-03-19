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
    public class GetProjectsQuery : IRequest<IQueryable<Project>>
    {

    }

    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IQueryable<Project>>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly ICurrentUserService _currentUserService;

        public GetProjectsQueryHandler(IApplicationDbContext ctx ,ICurrentUserService currentUserService)
        {
            _ctx = ctx;
            _currentUserService = currentUserService;
        }

        public Task<IQueryable<Project>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var output = _ctx.Projects
                .AsNoTracking()
                .Where(x => x.Members.Select(y => y.UserId).Contains(_currentUserService.UserId));

            return Task.FromResult(output);
        }
    }
}
