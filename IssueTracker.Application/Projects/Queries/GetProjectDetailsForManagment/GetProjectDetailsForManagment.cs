using IssueTracker.Application.Common.AccessPolicies;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment
{
    public class GetProjectDetailsForManagment : IRequest<ProjectManagmentDto>
    {
        public int ProjectId { get; set; }
    }

    public class GetProjectDetailsForManagmentQueryHandler : IRequestHandler<GetProjectDetailsForManagment, ProjectManagmentDto>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IUserService _userService;

        public GetProjectDetailsForManagmentQueryHandler(IApplicationDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
        }

        public async Task<ProjectManagmentDto> Handle(GetProjectDetailsForManagment request, CancellationToken cancellationToken)
        {
            var entity = _ctx.Projects
                .Include(x => x.Members)
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId).GetAwaiter().GetResult()
                .ApplyPolicy(new ProjectCanBeAccessedOnlyByMember(),_userService.GetCurrentUserId());

            if (entity is null)
                throw new NotFoundException(nameof(Project), request.ProjectId.ToString());

            var allUsers = _userService.GetAllUsers();
            var otherUsers = allUsers.ExceptBy(entity.Members.Select(x => x.UserId), allUsers => allUsers.UserId);

            return new ProjectManagmentDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Members = await entity.Members.SyncMembersWithUsers(_userService),
                OtherUsers = otherUsers
            };
        }
    }
}
