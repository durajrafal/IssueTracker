using IssueTracker.Application.Common.AccessPolicies;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Issues.Commands.UpdateIssue
{
    public class UpdateIssue : IRequest<int>, IHasTitle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; }
        public WorkingStatus Status { get; set; }
        public int ProjectId { get; set; }
        public IEnumerable<Member> Members { get; set; }
    }

    public class UpdateIssueHandler : IRequestHandler<UpdateIssue, int>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IUserService _userService;

        public UpdateIssueHandler(IApplicationDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
        }

        public async Task<int> Handle(UpdateIssue request, CancellationToken cancellationToken)
        {
            var entity = _ctx.Issues
                .Include(x => x.Members)
                .FirstOrDefaultAsync(x => x.Id == request.Id).GetAwaiter().GetResult()
                .ApplyPolicy(new IssueCanBeAccessedOnlyByProjectMember(_ctx),_userService.GetCurrentUserId());

            if (entity == null)
                throw new NotFoundException(nameof(Issue), request.Id.ToString());
            await entity.Members.SyncMembersWithUsers(_userService);

            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.Priority = request.Priority;
            entity.Status = request.Status;
            entity.UpdateMembers(request.Members, _userService.GetCurrentUserId());

            return await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
