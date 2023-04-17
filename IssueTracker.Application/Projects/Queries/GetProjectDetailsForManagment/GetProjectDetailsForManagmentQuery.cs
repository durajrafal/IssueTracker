using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment
{
    public class GetProjectDetailsForManagmentQuery : IRequest<ProjectManagmentDto>
    {
        public int ProjectId { get; set; }
    }

    public class GetProjectDetailsForManagmentQueryHandler : IRequestHandler<GetProjectDetailsForManagmentQuery, ProjectManagmentDto>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IUserService _userService;

        public GetProjectDetailsForManagmentQueryHandler(IApplicationDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
        }

        public async Task<ProjectManagmentDto> Handle(GetProjectDetailsForManagmentQuery request, CancellationToken cancellationToken)
        {
            var entity = new ProjectManagmentDto();

            var project = await _ctx.Projects
                .Include(x => x.Members)
                .FirstAsync(x => x.Id == request.ProjectId);
            entity.Title = project.Title;
            entity.Members = project.Members;
            foreach (var member in entity.Members)
            {
                member.User = await _userService.GetUserByIdAsync(member.UserId);
            }
            var allUsers = await _userService.GetAllUsersAsync();
            entity.OtherUsers = allUsers.ExceptBy(project.Members.Select(x => x.UserId), allUsers => allUsers.UserId);

            return entity;
        }
    }
}
