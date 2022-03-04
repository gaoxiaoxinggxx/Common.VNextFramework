using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.VNextFramework.Auditing
{
    public interface IAuditingStore
    {
        Task SaveAsync(AuditLogInfo auditInfo);
    }
}
