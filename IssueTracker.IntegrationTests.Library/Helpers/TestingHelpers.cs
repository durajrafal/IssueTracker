using IssueTracker.Infrastructure.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.IntegrationTests.Library.Helpers
{
    public class TestingHelpers
    {
        private IServiceScopeFactory _scopeFactory;

        public TestingHelpers(WebApplicationFactory<Program> factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        public async Task MediatorSendAsync(IRequest request)
        {
            var mediator = GetSenderFromServices();
            await mediator.Send(request);
        }

        public async Task<TResponse> MediatorSendAsync<TResponse>(IRequest<TResponse> request)
        {
            var mediator = GetSenderFromServices();
            return await mediator.Send(request);
        }

        public async Task ActionDatabaseAsync(Action<AppDbContext> action)
        {
            using (var ctx = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>())
            {
                action(ctx);
                await ctx.SaveChangesAsync();
            }
        }

        public TResult FuncDatabase<TResult>(Func<AppDbContext, TResult> func)
        {
            using (var ctx = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>())
            {
                return func(ctx);
            }
        }

        private ISender GetSenderFromServices()
        {
            var scope = _scopeFactory.CreateScope();

            return scope.ServiceProvider.GetRequiredService<ISender>();
        }
    }


}
