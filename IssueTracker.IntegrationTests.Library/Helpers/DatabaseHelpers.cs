using IssueTracker.Infrastructure.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.IntegrationTests.Library.Helpers
{
    public class DatabaseHelpers
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DatabaseHelpers(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task ActionAsync(Action<AppDbContext> action)
        {
            using (var ctx = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>())
            {
                action(ctx);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task ActionAsync<TDbContext>(Action<TDbContext> action) where TDbContext : DbContext
        {
            using (var ctx = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TDbContext>())
            {
                action(ctx);
                await ctx.SaveChangesAsync();
            }
        }

        public TResult Func<TResult>(Func<AppDbContext, TResult> func)
        {
            using (var ctx = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>())
            {
                return func(ctx);
            }
        }

        public TResult Func<TDbContext,TResult>(Func<TDbContext, TResult> func) where TDbContext : DbContext
        {
            using (var ctx = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TDbContext>())
            {
                return func(ctx);
            }
        }
    }


}
