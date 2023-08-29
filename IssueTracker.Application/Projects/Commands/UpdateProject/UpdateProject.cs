using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Project entity;

            try
            {
                entity = _ctx.Projects
                .Include(x => x.Members)
                .First(x => x.Id == request.Id);
            }
            catch (Exception ex)
            {
                throw new NotFoundException(nameof(Project), request.Id.ToString(), ex);
            }

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
