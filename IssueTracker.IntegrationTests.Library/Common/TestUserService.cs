using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.IntegrationTests.Library.Common
{
    public partial class TestUserService : ICurrentUserService
    {
        private readonly string _userId;

        public string UserId => _userId;

        public TestUserService()
        {
            _userId = Guid.NewGuid().ToString();
        }
    }
}
