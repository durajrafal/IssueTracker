using IssueTracker.Application.Common.AccessPolicies;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Projects.Commands.DeleteProject
{
    public class DeleteProject : IRequest<int>, IHasTitle
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
    }

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProject, int>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IUserService _userService;

        public DeleteProjectCommandHandler(IApplicationDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
        }

        public async Task<int> Handle(DeleteProject request, CancellationToken cancellationToken)
        {
            var entity = _ctx.Projects
                .Include(x => x.Members)
                .FirstOrDefault(x => x.Id == request.ProjectId)
                .ApplyPolicy(new ProjectCanBeAccessedOnlyByMember(),_userService.GetCurrentUserId());

            if (entity is null)
                throw new NotFoundException(nameof(Project), request.ProjectId.ToString());

            entity.Members.ToList().ForEach(x => _userService.RemoveProjectAccessClaimFromUserAsync(x.UserId, entity.Id));
            _ctx.Projects.Remove(entity);
            return await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
