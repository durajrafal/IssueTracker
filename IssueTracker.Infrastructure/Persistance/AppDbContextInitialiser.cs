using IssueTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Infrastructure
{
    public class AppDbContextInitialiser
    {
        private readonly AppDbContext _ctx;

        public AppDbContextInitialiser(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_ctx.Database.IsSqlServer())
                {
                    await _ctx.Database.MigrateAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
