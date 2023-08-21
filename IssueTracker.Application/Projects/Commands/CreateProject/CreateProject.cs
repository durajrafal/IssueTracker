using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;

namespace IssueTracker.Application.Projects.Commands.CreateProject
{
    public class CreateProject : IRequest<int>, IHasTitle
    {
        public string Title { get; set; }
    }

    public class CreateProjectCommandHandler : IRequestHandler<CreateProject, int>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;

        public CreateProjectCommandHandler(IApplicationDbContext ctx, ICurrentUserService currentUserService, IUserService userService)
        {
            _ctx = ctx;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        public async Task<int> Handle(CreateProject request, CancellationToken cancellationToken)
        {
            var entity = new Project
            {
                Title = request.Title,
            };
            var currentUserId = _currentUserService.UserId;
            entity.Members.AddNewOrExistingMember(_ctx.Members, currentUserId);
            _ctx.Projects.Add(entity);
            await _ctx.SaveChangesAsync(cancellationToken);

            await _userService.AddProjectAccessClaimToUserAsync(currentUserId, entity.Id);

            return entity.Id;
        }
    }
}
