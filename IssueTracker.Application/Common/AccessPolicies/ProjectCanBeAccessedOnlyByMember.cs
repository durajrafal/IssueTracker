using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;

namespace IssueTracker.Application.Common.AccessPolicies
{
    internal class ProjectCanBeAccessedOnlyByMember : IAccessPolicy<Project>
    {
        public Project? Apply(Project entity, string userId)
        {
            if (entity.Members.Any(x => x.UserId == userId))
                return entity;
            return null;
        }
    }
}
