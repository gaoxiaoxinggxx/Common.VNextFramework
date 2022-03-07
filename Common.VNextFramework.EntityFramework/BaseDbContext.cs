using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.VNextFramework.Auditing;
using Common.VNextFramework.EntityFramework.EntityHistory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.VNextFramework.EntityFramework
{
    public abstract class BaseDbContext<TDbContext> : DbContext
        where TDbContext:DbContext
    {

        public IAuditingManager AuditingManager { get; }
        public IServiceProvider ServiceProvider { get;  }
        
        public IEntityHistoryHelper EntityHistoryHelper;

        protected BaseDbContext(DbContextOptions<TDbContext> options, IServiceProvider serviceProvider) : base(options)
        {
            ServiceProvider = serviceProvider;

            //TODO:change to lazy get
            EntityHistoryHelper = (IEntityHistoryHelper)serviceProvider.GetService(typeof(IEntityHistoryHelper));
            AuditingManager = (IAuditingManager)serviceProvider.GetService(typeof(IAuditingManager));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

       
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            
            try
            {
                var auditLog = AuditingManager?.Current?.Log;

                List<EntityChangeInfo> entityChangeList = null;
                if (auditLog != null)
                {
                    entityChangeList = EntityHistoryHelper.CreateChangeInfos(ChangeTracker.Entries().ToList());
                }

                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

                if (auditLog != null)
                {
                    EntityHistoryHelper.UpdateChangeList(entityChangeList);
                    auditLog.EntityChanges.AddRange(entityChangeList);
                    //Logger<>.LogDebug($"Added {entityChangeList.Count} entity changes to the current audit log");
                }

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }
    }
}
