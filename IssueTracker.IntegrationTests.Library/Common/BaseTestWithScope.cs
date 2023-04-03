using IssueTracker.IntegrationTests.Library.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IssueTracker.IntegrationTests.Library.Common
{
    public class BaseTestWithScope : BaseTest
    {
        public readonly IServiceScopeFactory _scopeFactory;

        public BaseTestWithScope(CustomWebApplicationFactory factory)
            :base(factory)
        {
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        public BaseTestWithScope()
            :base()
        {
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        }
    }
}
