using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Application.IntegrationTests
{
    public class TestsWithMediatorFixture : TestsFixture
    {
        public TestsWithMediatorFixture()
            : base()
        {
        }

        public TestsWithMediatorFixture(CustomWebApplicationFactory factory) 
            : base(factory)
        {
        }

        public ISender Mediator { get => ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ISender>(); }
    }
}
