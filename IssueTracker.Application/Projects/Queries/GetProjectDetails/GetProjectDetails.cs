using IssueTracker.Application.Common.AccessPolicies;
using IssueTracker.Application.Common.Exceptions;
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
        private readonly ICurrentUserService _currentUserService;

        public GetProjectDetailsQueryHandler(IApplicationDbContext ctx, IUserService userService, ICurrentUserService currentUserService)
        {
            _ctx = ctx;
            _userService = userService;
            _currentUserService = currentUserService;
        }

        public async Task<ProjectDto> Handle(GetProjectDetails request, CancellationToken cancellationToken)
        {
            var entity = _ctx.Projects
                .AsNoTracking()
                .Include(x => x.Members)
                .Include(x => x.Issues)
                .ThenInclude(y => y.Members)
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId).GetAwaiter().GetResult()
                .ApplyPolicy(new ProjectCanBeAccessedOnlyByMember(), _currentUserService.UserId);

            if (entity is null)
                throw new NotFoundException(nameof(Project), request.ProjectId.ToString());

            foreach (var issue in entity.Issues)
            {
                await issue.Members.SyncMembersWithUsers(_userService);
            }

            return new ProjectDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Members = await entity.Members.SyncMembersWithUsers(_userService),
                Issues = entity.Issues,
                Created = entity.Created,
                CreatedByUser = _userService.GetUserByIdAsync(entity.CreatedBy).GetAwaiter().GetResult()!,
                LastModified = entity.LastModified,
                LastModifiedByUser = _userService.GetUserByIdAsync(entity.CreatedBy).GetAwaiter().GetResult()
            };
        }
    }
}
