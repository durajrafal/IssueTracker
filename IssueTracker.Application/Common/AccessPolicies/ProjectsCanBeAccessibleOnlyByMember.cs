using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;

namespace IssueTracker.Application.Common.AccessPolicies
{
    internal class ProjectsCanBeAccessibleOnlyByMember : IAccessPolicy<IQueryable<Project>>
    {
        public IQueryable<Project>? Apply(IQueryable<Project> entity, string userId)
        {
            return entity.Where(x => x.Members.Any(m => m.UserId == userId));
        }
    }
}
