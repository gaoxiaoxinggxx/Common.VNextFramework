using System;
using System.Collections.Generic;
using System.Text;

namespace Common.VNextFramework.Auditing
{
    public abstract class AuditLogContributor
    {
        public virtual void PreContribute(AuditLogContributionContext context)
        {
        }

        public virtual void PostContribute(AuditLogContributionContext context)
        {
        }
    }
    public class AuditLogContributionContext
    {
        public IServiceProvider ServiceProvider { get; }

        public AuditLogInfo AuditInfo { get; }

        public AuditLogContributionContext(IServiceProvider serviceProvider, AuditLogInfo auditInfo)
        {
            ServiceProvider = serviceProvider;
            AuditInfo = auditInfo;
        }
    }
}
