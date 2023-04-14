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
        public DbSet<ProjectMember> ProjectsMembers { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueMember> IssueMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProjectMember>().HasKey(x => new { x.ProjectId, x.UserId });
            modelBuilder.Entity<IssueMember>().HasKey(x => new { x.IssueId, x.UserId });
            modelBuilder.Entity<ProjectMember>().Ignore(x => x.User);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
