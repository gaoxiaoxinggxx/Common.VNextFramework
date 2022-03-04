using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Common.VNextFramework.Auditing
{
    public class SimpleLogAuditingStore : IAuditingStore
    {
        public ILogger<SimpleLogAuditingStore> Logger { get; set; }

        public SimpleLogAuditingStore(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<SimpleLogAuditingStore>();
        }

        public Task SaveAsync(AuditLogInfo auditInfo)
        {
            Logger.LogInformation(auditInfo.ToString());
            return Task.FromResult(0);
        }
    }
}
