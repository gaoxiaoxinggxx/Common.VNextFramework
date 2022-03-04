using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Common.VNextFramework.Auditing
{
    public interface IAuditingOptionsExtension
    {
        IServiceCollection AddServices(IServiceCollection services);
    }
}
