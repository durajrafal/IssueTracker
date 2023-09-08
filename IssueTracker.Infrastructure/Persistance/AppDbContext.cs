using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Common;
using IssueTracker.Domain.Entities;
using IssueTracker.Infrastructure.Persistance.Interceptors;
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
        private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

        public AppDbContext(DbContextOptions<AppDbContext> options, AuditableEntityInterceptor auditableEntityInterceptor) : base(options)
        {
            _auditableEntityInterceptor = auditableEntityInterceptor;
        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Issue> Issues => Set<Issue>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Member>().Ignore(x => x.User);
            modelBuilder.Entity<Project>()
                .HasMany(e => e.Members)
                .WithMany(e => e.Projects)
                .UsingEntity("ProjectsMembers");
            
            modelBuilder.Entity<Issue>()
                .HasMany(e => e.Members)
                .WithMany(e => e.Issues)
                .UsingEntity("IssuesMembers");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
