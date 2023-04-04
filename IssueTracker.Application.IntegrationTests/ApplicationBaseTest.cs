using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Application.IntegrationTests
{
    public class ApplicationBaseTest : BaseTest
    {
        public ApplicationBaseTest()
            : base()
        {
        }

        public ApplicationBaseTest(CustomWebApplicationFactory factory) 
            : base(factory)
        {
        }

        public ISender Mediator { get => ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ISender>(); }
    }
}
