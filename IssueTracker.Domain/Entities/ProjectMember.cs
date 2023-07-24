using IssueTracker.Domain.Models;

namespace IssueTracker.Domain.Entities
{
    public class ProjectMember : Member, IEquatable<ProjectMember>
    {
        public int ProjectId { get; set; }

        public bool Equals(ProjectMember other)
        {
            return UserId == other.UserId && ProjectId == other.ProjectId;
        }

        public override bool Equals(object obj) => Equals(obj as ProjectMember);
        public override int GetHashCode() => (UserId, ProjectId).GetHashCode();
    }
}
