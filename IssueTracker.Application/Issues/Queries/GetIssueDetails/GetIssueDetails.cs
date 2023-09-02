using IssueTracker.Application.Common.AccessPolicies;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Issues.Queries.GetIssueDetails
{
    public record GetIssueDetails(int Id) : IRequest<IssueDto>;

    public class GetIssueDetailsHandler : IRequestHandler<GetIssueDetails, IssueDto>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IUserService _userService;

        public GetIssueDetailsHandler(IApplicationDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
        }

        public async Task<IssueDto> Handle(GetIssueDetails request, CancellationToken cancellationToken)
        {
            var entity = _ctx.Issues
                .Include(x => x.Members)
                .Include(x => x.Project)
                .FirstOrDefaultAsync(x => x.Id == request.Id).GetAwaiter().GetResult()
                .ApplyPolicy(new IssueCanBeAccessedOnlyByProjectMember(_ctx), _userService.GetCurrentUserId());

            if (entity is null)
                throw new NotFoundException(nameof(Issue), request.Id.ToString());

            await entity.Members.SyncMembersWithUsers(_userService);

            return new IssueDto()
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Priority = entity.Priority,
                Status = entity.Status,
                Members = entity.Members,
                Project = entity.Project
            };
        }
    }
}
