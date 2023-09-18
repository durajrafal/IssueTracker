using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Common.AccessPolicies
{
    internal class IssueCanBeAccessedOnlyByProjectMember : IAccessPolicy<Issue>
    {
        private readonly IApplicationDbContext _ctx;

        internal IssueCanBeAccessedOnlyByProjectMember(IApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Issue? Apply(Issue entity, string userId)
        {
            var project = _ctx.Projects
                .AsNoTracking()
                .Include(x => x.Members)
                .FirstOrDefault(x => x.Id == entity.ProjectId)
                .ApplyPolicy(new ProjectCanBeAccessedOnlyByMember(), userId);

            if (project is null)
                return null;

            return entity;
        }
    }
}
