using IssueTracker.Application.Common.Interfaces;

namespace IssueTracker.Application.Common.Helpers
{
    public static class PolicyExecutor
    {

        public static TEntity? ApplyPolicy<TEntity>(this TEntity? entity, IAccessPolicy<TEntity> policy, string userId)
        {
            if (entity is null)
                return default;
            var output = policy.Apply(entity, userId);
            if (output is null)
                throw new UnauthorizedAccessException();
            return output;
        }
    }
}
