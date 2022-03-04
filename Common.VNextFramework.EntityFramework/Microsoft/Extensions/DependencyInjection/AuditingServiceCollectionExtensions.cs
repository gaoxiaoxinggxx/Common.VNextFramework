using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.VNextFramework.Auditing;
using Common.VNextFramework.EntityFramework.EntityHistory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuditLoginEntityFrameworkCoreServiceCollectionExtensions
    {
        public static AuditingOptions UseEntityFrameworkEntityHistory(this AuditingOptions options)
        {
            options.Extensions.Add(new EntityFrameworkEntityHistoryAuditingOptionsExtension());

            return options;
        }
    }

    public class EntityFrameworkEntityHistoryAuditingOptionsExtension : IAuditingOptionsExtension
    {
        public IServiceCollection AddServices(IServiceCollection services)
        {
            services.TryAddSingleton<IEntityHistoryHelper, EntityHistoryHelper>();
            return services;
        }
    }
}
