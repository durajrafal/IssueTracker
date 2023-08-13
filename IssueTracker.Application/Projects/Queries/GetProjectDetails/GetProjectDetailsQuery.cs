using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

            await entity.Members.PopulateMembersWithUsers(_userService);

            return entity;
        }
    }
}
