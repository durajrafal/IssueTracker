using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Infrastructure.Persistance
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Issue> Issues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Project>().HasMany(p => p.Members).WithMany(m => m.Projects);
            modelBuilder.Entity<Issue>().HasMany(i => i.Members).WithMany(m => m.Issues);
            //modelBuilder.Entity<Member>().HasKey(x => new { x.Projects, x.UserId });
            //modelBuilder.Entity<IssueMember>().HasKey(x => new { x.IssueId, x.UserId });
            modelBuilder.Entity<Member>().Ignore(x => x.User);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
