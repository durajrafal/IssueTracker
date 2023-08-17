using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Projects.Queries.GetProjectDetails
{
    public class GetProjectDetails : IRequest<ProjectDto>
    {
        public int ProjectId { get; set; }
    }

    public class GetProjectDetailsQueryHandler : IRequestHandler<GetProjectDetails, ProjectDto>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IUserService _userService;

        public GetProjectDetailsQueryHandler(IApplicationDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
        }

        public async Task<ProjectDto> Handle(GetProjectDetails request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Projects
                .Include(x => x.Members)
                .Include(x => x.Issues)
                .ThenInclude(y => y.Members)
                .FirstAsync(x => x.Id == request.ProjectId);

            foreach (var issue in entity.Issues)
            {
                await issue.Members.SyncMembersWithUsers(_userService);
            }

            return new ProjectDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Members = await entity.Members.SyncMembersWithUsers(_userService),
                Issues = entity.Issues
            };
        }
    }
}
