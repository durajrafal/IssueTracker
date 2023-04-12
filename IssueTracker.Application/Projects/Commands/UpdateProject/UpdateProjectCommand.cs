using IssueTracker.Application.Common.Exceptions;
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
    public class UpdateProjectCommand : IRequest<int>
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public IList<ProjectMember> Members { get; set; }
    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, int>
    {
        private readonly IApplicationDbContext _ctx;

        public UpdateProjectCommandHandler(IApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<int> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            Project entity;

            try
            {
                entity = _ctx.Projects
                .Include(x => x.Members)
                .First(x => x.Id == request.ProjectId);
            }
            catch (Exception ex)
            {
                throw new NotFoundException(nameof(Project), request.ProjectId.ToString(), ex);
            }

            entity.Title = request.Title;
            var membersToAdd = request.Members.Except(entity.Members).ToList();
            membersToAdd.ForEach(x => entity.Members.Add(x));
            var membersToRemove = entity.Members.Except(request.Members).ToList();
            membersToRemove.ForEach(x => entity.Members.Remove(x));

            return await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
