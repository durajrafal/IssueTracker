using IssueTracker.Application.Common.AccessPolicies;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Projects.Commands.UpdateProject
{
    public class UpdateProject : IRequest<int>, IHasTitle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<Member> Members { get; set; }
    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProject, int>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IUserService _userService;

        public UpdateProjectCommandHandler(IApplicationDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
        }

        public async Task<int> Handle(UpdateProject request, CancellationToken cancellationToken)
        {
            var entity = _ctx.Projects
                .Include(x => x.Members)
                .FirstOrDefaultAsync(x => x.Id == request.Id).GetAwaiter().GetResult()
                .ApplyPolicy(new ProjectCanBeAccessedOnlyByMember(), _userService.GetCurrentUserId());

            if(entity is null)
                throw new NotFoundException(nameof(Project), request.Id.ToString());

            entity.Title = request.Title;

            var membersToAdd = request.Members.Except(entity.Members).ToList();
            foreach (var member in membersToAdd)
            {
                entity.Members.AddNewOrExistingMember(_ctx.Members, member.UserId);
                await _userService.AddProjectAccessClaimToUserAsync(member.UserId, entity.Id);
            }

            var membersToRemove = entity.Members.Except(request.Members).ToList();
            foreach (var member in membersToRemove)
            {
                entity.Members.Remove(member);
                await _userService.RemoveProjectAccessClaimFromUserAsync(member.UserId, entity.Id);
            }

            return await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
