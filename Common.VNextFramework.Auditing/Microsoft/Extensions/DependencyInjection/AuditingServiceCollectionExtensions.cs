using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.VNextFramework.Auditing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuditingServiceCollectionExtensions
    {
        public static AuditingOptions UseAuditingToLoggingStore(this AuditingOptions options)
        {
            options.Extensions.Add(new LoggingAuditingStoreOptionsExtension());
            return options;
        }

    }
}
