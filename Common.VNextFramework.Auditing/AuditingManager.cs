using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.VNextFramework.Auditing
{
    public class AuditingManager : IAuditingManager
    {
        private static AsyncLocal<IAuditLogScope> _scope = new AsyncLocal<IAuditLogScope>();
        protected IServiceProvider ServiceProvider { get; }
        private readonly IAuditingStore _auditingStore;
        protected AuditingOptions Options { get; }
        private readonly ILogger _logger;

        public AuditingManager(
            IAuditingStore auditingStore,
            IOptions<AuditingOptions> options, 
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            _auditingStore = auditingStore;
            Options = options.Value;
            ServiceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger(nameof(AuditingManager));
        }

        public IAuditLogScope Current => _scope.Value;
        public IAuditLogSaveHandle BeginScope()
        {
            if (Current == null)
            {
                var auditLog = CreateAuditLogInfo();
                lock (this)
                {
                    if (Current == null)
                    {
                        _scope.Value = new AuditLogScope(auditLog);
                    }
                }
            }

            return new DisposableSaveHandle(this,  Current.Log, Stopwatch.StartNew());
        }


        public virtual AuditLogInfo CreateAuditLogInfo()
        {
            var auditInfo = new AuditLogInfo
            {
                ApplicationName = Options.ApplicationName,
                ExecutionTime = DateTime.Now
            };

            ExecutePreContributors(auditInfo);

            return auditInfo;
        }

        protected virtual void ExecutePreContributors(AuditLogInfo auditLogInfo)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = new AuditLogContributionContext(scope.ServiceProvider, auditLogInfo);

                foreach (var contributor in Options.Contributors)
                {
                    try
                    {
                        contributor.PreContribute(context);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, ex.Message);
                    }
                }
            }
        }

        protected virtual void ExecutePostContributors(AuditLogInfo auditLogInfo)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = new AuditLogContributionContext(scope.ServiceProvider, auditLogInfo);

                foreach (var contributor in Options.Contributors)
                {
                    try
                    {
                        contributor.PostContribute(context);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, ex.Message);
                    }
                }
            }
        }

        protected virtual void BeforeSave(DisposableSaveHandle saveHandle)
        {
            saveHandle.StopWatch.Stop();
            saveHandle.AuditLog.ExecutionDuration = Convert.ToInt32(saveHandle.StopWatch.Elapsed.TotalMilliseconds);
            ExecutePostContributors(saveHandle.AuditLog);
            MergeEntityChanges(saveHandle.AuditLog);
        }

        protected virtual void MergeEntityChanges(AuditLogInfo auditLog)
        {
            var changeGroups = auditLog.EntityChanges
                .Where(e => e.ChangeType == EntityChangeType.Updated)
                .GroupBy(e => new { e.EntityTypeFullName, e.EntityId })
                .ToList();

            foreach (var changeGroup in changeGroups)
            {
                if (changeGroup.Count() <= 1)
                {
                    continue;
                }

                var firstEntityChange = changeGroup.First();

                foreach (var entityChangeInfo in changeGroup)
                {
                    if (entityChangeInfo == firstEntityChange)
                    {
                        continue;
                    }

                    firstEntityChange.Merge(entityChangeInfo);

                    auditLog.EntityChanges.Remove(entityChangeInfo);
                }
            }
        }


        protected virtual async Task SaveAsync(DisposableSaveHandle saveHandle)
        {
            BeforeSave(saveHandle);

            await _auditingStore.SaveAsync(saveHandle.AuditLog);
        }

        protected class DisposableSaveHandle : IAuditLogSaveHandle
        {
            public AuditLogInfo AuditLog { get; }
            public Stopwatch StopWatch { get; }

            private readonly AuditingManager _auditingManager;
            //private readonly IDisposable _scope;

            public DisposableSaveHandle(
                AuditingManager auditingManager,
                //IDisposable scope,
                AuditLogInfo auditLog,
                Stopwatch stopWatch)
            {
                _auditingManager = auditingManager;
                //_scope = scope;
                AuditLog = auditLog;
                StopWatch = stopWatch;
            }

            public async Task SaveAsync()
            {
                await _auditingManager.SaveAsync(this);
            }

            public void Dispose()
            {
                AuditingManager._scope.Value = null;
            }
        }
    }
}