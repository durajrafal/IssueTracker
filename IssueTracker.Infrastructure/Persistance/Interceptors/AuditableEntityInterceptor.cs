using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Common;
using IssueTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace IssueTracker.Infrastructure.Persistance.Interceptors
{
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditableEntityInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
            {
                var dateTimeNow = DateTime.UtcNow;
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.Created = dateTimeNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedById = _currentUserService.UserId;
                    entry.Entity.LastModified = dateTimeNow;
                    AddAuditEventsForModifiedProperties(entry, dateTimeNow);
                }
            }
        }

        private void AddAuditEventsForModifiedProperties(EntityEntry<IAuditableEntity> entry, DateTime timestamp)
        {
            foreach (var property in entry.Properties.Where(x => x.IsModified))
            {
                var auditEvent = new AuditEvent
                {
                    PropertyName = property.Metadata.Name,
                    OldValue = JsonSerializer.Serialize(property.OriginalValue),
                    NewValue = JsonSerializer.Serialize(property.CurrentValue),
                    ModifiedById = _currentUserService.UserId,
                    Timestamp = timestamp
                };
                entry.Entity.AuditEvents.Add(auditEvent);
            }
        }
    }
}
