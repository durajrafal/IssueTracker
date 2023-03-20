using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.IntegrationTests.Common
{
    public class BaseTest : IClassFixture<CustomWebApplicationFactory>
    {
        internal readonly CustomWebApplicationFactory _factory;
        internal readonly TestingHelpers _testing;

        public BaseTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _testing = new TestingHelpers(_factory);
        }
    }
}
