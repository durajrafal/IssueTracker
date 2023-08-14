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

        public CreateProjectCommandHandler(IApplicationDbContext ctx, ICurrentUserService currentUserService)
        {
            _ctx = ctx;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(CreateProject request, CancellationToken cancellationToken)
        {
            var entity = new Project
            {
                Title = request.Title,
            };
            entity.AddNewOrExistingMember(_ctx.Members, _currentUserService.UserId);

            _ctx.Projects.Add(entity);
            await _ctx.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
