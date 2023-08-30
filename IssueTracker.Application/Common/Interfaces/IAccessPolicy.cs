namespace IssueTracker.Application.Common.Interfaces
{
    internal interface IAccessPolicy<T>
    {
        T? Apply(T accessedObject, string userId);
    }
}
