using IssueTracker.IntegrationTests.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IssueTracker.IntegrationTests.Library.Common
{
    public class BaseTest : IClassFixture<CustomWebApplicationFactory>
    {
        public readonly CustomWebApplicationFactory _factory;
        public readonly TestingHelpers _testing;

        public BaseTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _testing = new TestingHelpers(_factory);
        }

        public BaseTest()
        {
            _factory = new CustomWebApplicationFactory();
            _testing = new TestingHelpers(_factory);
        }
    }
}
