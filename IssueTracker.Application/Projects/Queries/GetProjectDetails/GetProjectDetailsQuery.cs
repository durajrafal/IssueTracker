using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Projects.Queries.GetProjectDetails
{
    public class GetProjectDetailsQuery : IRequest<Project>
    {
        public int ProjectId { get; set; }
    }

    public class GetProjectDetailsQueryHandler : IRequestHandler<GetProjectDetailsQuery, Project>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IUserService _userService;

        public GetProjectDetailsQueryHandler(IApplicationDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
        }

        public async Task<Project> Handle(GetProjectDetailsQuery request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Projects
                .Include(x => x.Members)
                .Include(x => x.Issues)
                .ThenInclude(y => y.Members)
                .FirstAsync(x => x.Id == request.ProjectId);

            entity.Members.ToList().ForEach(async x => x.User = await _userService.GetUserByIdAsync(x.UserId));

            return entity;
        }
    }
}
