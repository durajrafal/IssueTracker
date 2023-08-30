using IssueTracker.Application.Common.Interfaces;

namespace IssueTracker.Application.Common.Helpers
{
    public static class PolicyExecutor
    {

        public static TEntity ApplyPolicy<TEntity>(this TEntity entity, IAccessPolicy<TEntity> policy, string userId)
        {
            var output = policy.Apply(entity, userId);
            if (output is null)
                throw new UnauthorizedAccessException();
            return output;
        }
        public static IQueryable<TEntity> ApplyPolicy<TEntity>(this IQueryable<TEntity> queryable, IAccessPolicy<TEntity> policy, string userId)
        {
            return policy.Apply(queryable, userId);
        }
    }
}
