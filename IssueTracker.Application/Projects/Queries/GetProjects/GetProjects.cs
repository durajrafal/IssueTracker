using IssueTracker.Application.Common.AccessPolicies;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using MediatR;

namespace IssueTracker.Application.Projects.Queries.GetProjects
{
    public class GetProjects : IRequest<IEnumerable<ProjectSummaryDto>>
    {

    }

    public class GetProjectsQueryHandler : IRequestHandler<GetProjects, IEnumerable<ProjectSummaryDto>>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly ICurrentUserService _currentUserService;

        public GetProjectsQueryHandler(IApplicationDbContext ctx, ICurrentUserService currentUserService)
        {
            _ctx = ctx;
            _currentUserService = currentUserService;
        }

        public Task<IEnumerable<ProjectSummaryDto>> Handle(GetProjects request, CancellationToken cancellationToken)
        {
            var output = _ctx.Projects
                .ApplyPolicy(new ProjectCanBeAccessedOnlyByMember(), _currentUserService.UserId)
                .Select(x =>
                    new ProjectSummaryDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        NumberOfMembers = x.Members.Count,
                    }
                ).AsEnumerable();

            return Task.FromResult(output);
        }
    }
}
