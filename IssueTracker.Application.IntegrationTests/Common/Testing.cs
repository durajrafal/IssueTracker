using IssueTracker.Infrastructure.Persistance;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.IntegrationTests.Common
{
    public class Testing
    {
        private readonly CustomWebApplicationFactory _fixture;
        private IServiceScopeFactory _scopeFactory;

        public Testing()
        {
            _fixture = new();
            _scopeFactory = _fixture.Services.GetRequiredService<IServiceScopeFactory>();
        }

        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            var scope = _scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

            return await mediator.Send(request);
        }

        public void ActionDatabase(Action<AppDbContext> action)
        {
            using (var ctx = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>())
            {
                action(ctx);
            }
        }

        public TResult FuncDatabase<TResult>(Func<AppDbContext,TResult> func)
        {
            using (var ctx = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>())
            {
                return func(ctx);
            }
        }
    }


}
