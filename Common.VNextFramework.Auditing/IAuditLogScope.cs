using System;
using System.Collections.Generic;
using System.Text;

namespace Common.VNextFramework.Auditing
{
    public interface IAuditLogScope
    {
         AuditLogInfo Log { get; }
    }
    public class AuditLogScope : IAuditLogScope
    {
        public AuditLogInfo Log { get; }

        public AuditLogScope(AuditLogInfo log)
        {
            Log = log;
        }
    }
}
