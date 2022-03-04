using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.VNextFramework.Auditing
{
    public class LoggingAuditingStoreOptionsExtension : IAuditingOptionsExtension
    {
        public IServiceCollection AddServices(IServiceCollection services)
        {
            services.TryAddSingleton<IAuditingStore,SimpleLogAuditingStore>();
            return services;
        }
    }
}
