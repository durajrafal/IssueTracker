using IssueTracker.Application.Common.AccessPolicies;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Issues.Commands.DeleteIssue
{
    public record DeleteIssue(int Id) : IRequest<int>;

    public class DeleteIssueHandler : IRequestHandler<DeleteIssue, int>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly ICurrentUserService _currentUserService;

        public DeleteIssueHandler(IApplicationDbContext ctx, ICurrentUserService currentUserService)
        {
            _ctx = ctx;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(DeleteIssue request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Issues
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            entity.ApplyPolicy(new IssueCanBeAccessedOnlyByProjectMember(_ctx), _currentUserService.UserId);

            if (entity is null)
                throw new NotFoundException(nameof(Issue), request.Id.ToString());

            _ctx.Issues.Remove(entity);
            return await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
