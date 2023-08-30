using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Common.AccessPolicies
{
    public class ProjectCanBeAccessedOnlyByMember : IAccessPolicy<Project>
    {
        public Project? Apply(Project entity, string userId)
        {
            if (entity.Members.Any(x => x.UserId == userId))
                return entity;
            return null;
        }
    }
}
