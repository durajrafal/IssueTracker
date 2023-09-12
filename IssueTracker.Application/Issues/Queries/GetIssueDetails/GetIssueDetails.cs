using IssueTracker.Application.Common.AccessPolicies;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Common.Models;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Issues.Queries.GetIssueDetails
{
    public record GetIssueDetails(int Id, int PageNumber = 1, int PageSize = 5) : IRequest<IssueDto>;

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
                .Include(x => x.AuditEvents)
                .FirstOrDefaultAsync(x => x.Id == request.Id).GetAwaiter().GetResult()
                .ApplyPolicy(new IssueCanBeAccessedOnlyByProjectMember(_ctx), _userService.GetCurrentUserId());

            if (entity is null)
                throw new NotFoundException(nameof(Issue), request.Id.ToString());

            await entity.Members.SyncMembersWithUsers(_userService);
            await entity.Project.Members.SyncMembersWithUsers(_userService);
            await entity.AuditEvents.SeedWithUsersAsync(_userService);

            return new IssueDto()
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Priority = entity.Priority,
                Status = entity.Status,
                Members = entity.Members,
                Project = entity.Project,
                Audit = new AuditDto()
                {
                    Created = entity.Created,
                    CreatedByUser = _userService.GetUserByIdAsync(entity.CreatedBy).GetAwaiter().GetResult()!,
                    LastModified = entity.LastModified,
                    LastModifiedBy = _userService.GetUserByIdAsync(entity.LastModifiedById).GetAwaiter().GetResult(),
                    AuditEvents = PaginatedList<AuditEvent>.Create(entity.AuditEvents.OrderByDescending(x => x.Timestamp).AsQueryable(),
                    request.PageNumber, request.PageSize)
                }
            };
        }
    }
}
