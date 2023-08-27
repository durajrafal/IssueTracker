using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.IntegrationTests.Library.Common
{
    public partial class TestCurrentUserService : ICurrentUserService
    {
        private readonly string _userId;

        public string UserId => _userId;

        public TestCurrentUserService()
        {
            _userId = Guid.NewGuid().ToString();
        }
    }
}
