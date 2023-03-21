using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Projects.Queries.GetProjects
{
    public class GetProjectsQuery : IRequest<IQueryable<ProjectSummaryDto>>
    {

    }

    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IQueryable<ProjectSummaryDto>>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly ICurrentUserService _currentUserService;

        public GetProjectsQueryHandler(IApplicationDbContext ctx, ICurrentUserService currentUserService)
        {
            _ctx = ctx;
            _currentUserService = currentUserService;
        }

        public Task<IQueryable<ProjectSummaryDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var output = _ctx.Projects
                .Where(x =>
                    x.Members.Select(y => y.UserId).Contains(_currentUserService.UserId)
                )
                .Select(x =>
                    new ProjectSummaryDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        NumberOfMembers = x.Members.Count,
                    }
                );
                

            return Task.FromResult(output);
        }
    }
}
