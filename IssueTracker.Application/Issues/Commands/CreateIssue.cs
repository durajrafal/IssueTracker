using IssueTracker.Application.Common.AccessPolicies;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Enums;
using MediatR;

namespace IssueTracker.Application.Issues.Commands
{
    public class CreateIssue : IRequest<int>, IHasTitle
    {
        public string Title { get; set; }
        public int ProjectId { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; }
        public IEnumerable<Member> Members { get; set; }
    }

    public class CreateIssueHandler : IRequestHandler<CreateIssue, int>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly ICurrentUserService _currentUserService;

        public CreateIssueHandler(IApplicationDbContext ctx, ICurrentUserService currentUserService)
        {
            _ctx = ctx;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(CreateIssue request, CancellationToken cancellationToken)
        {
            var entity = new Issue
            {
                Title = request.Title,
                ProjectId = request.ProjectId,
                Description = request.Description,
                Priority = request.Priority,
            };

            entity.ApplyPolicy(new IssueCanBeAccessedOnlyByProjectMember(_ctx), _currentUserService.UserId);

            request.Members?.ToList().ForEach(x => entity.Members.AddNewOrExistingMember(_ctx.Members, x.UserId));

            await _ctx.Issues.AddAsync(entity);

            await _ctx.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
